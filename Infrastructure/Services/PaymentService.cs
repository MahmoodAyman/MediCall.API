using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Core.DTOs.Payment;
using Core.Enums;
using Core.Interface;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Infrastructure.Data;

namespace Infrastructure.Services;

public class PaymentService(IConfiguration _config, MediCallContext _context, HttpClient _httpClient) : IPaymentService
{
    public async Task<PaymentResponseDto> CreateOrUpdatePayment(int visitId)
    {
        // Get Visit
        var visit = await _context.Visits
                    .Include(v => v.Patient)
                    .Include(v => v.Services)
                    .Include(v => v.Nurse)
                    .FirstOrDefaultAsync(v => v.Id == visitId);

        // Check Visit existed?
        if (visit == null)
        {
            throw new ArgumentException($"There is no such a visit with this Id {visitId}");
        }

        // Check if the visitPayment already Happened before!
        var existingPayment = await _context.Payments.FirstOrDefaultAsync(p => p.VisitId == visitId);
        if (existingPayment != null)
        {
            return new PaymentResponseDto
            {
                Id = existingPayment.Id,
                TransactionReference = existingPayment.TransactionReference,
                Status = existingPayment.Status,
                Amount = existingPayment.Amount
            };
        }

        // Create new Payment request
        var request = new HttpRequestMessage(HttpMethod.Post, "https://accept.paymob.com/api/acceptance/payment_keys");

        var apiKey = _config["PaymentGateways:Paymob:ApiKey"];
        var integrationId = _config["PaymentGateways:Paymob:PaymentIntegrationId"];
        var iframeId = _config["PaymentGateways:Paymob:IframeId"];

        if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(integrationId) || string.IsNullOrEmpty(iframeId))
        {
            throw new InvalidOperationException("Paymob configuration is missing");
        }

        request.Headers.Add("Authorization", $"Bearer {apiKey}");

        decimal totalAmount = visit.CalculateTotalCost();

        // payment request payload: 
        var paymentRequest = new
        {
            auth_token = apiKey,
            amount_cents = (int)(totalAmount * 100),
            expiration = 3600,
            order_id = Guid.NewGuid().ToString(),
            billing_data = new
            {
                apartment = "NA",
                email = visit.Patient.Email,
                floor = "NA",
                first_name = visit.Patient.FirstName,
                street = "NA",
                building = "NA",
                phone_number = visit.Patient.PhoneNumber,
                shipping_method = "NA",
                postal_code = "NA",
                city = "Cairo",
                country = "EG",
                last_name = visit.Patient.LastName,
                state = "NA"
            },
            currency = "EGP",
            integration_id = int.Parse(integrationId),
            lock_order_when_paid = true,
            extra = new
            {
                visit_id = visit.Id
            }
        };

        var jsonContent = JsonSerializer.Serialize(paymentRequest);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        request.Content = content;

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var responseData = JsonSerializer.Deserialize<PaymentKeyResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (responseData == null || string.IsNullOrEmpty(responseData.Token))
        {
            throw new Exception("Failed to get payment token from payment gateway");
        }

        // Create payment iframe URL
        var paymentUrl = $"https://accept.paymobsolutions.com/api/acceptance/iframes/{iframeId}?payment_token={responseData.Token}";

        var payment = new Payment
        {
            PaymentMethod = PaymentMethod.CreditCard,
            PaymentDate = DateTime.Now,
            Status = PaymentStatus.Pending,
            TransactionReference = responseData.Order_id,
            VisitId = visit.Id,
            Visit = visit,
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();

        visit.Status = VisitStatus.PendingPayment;
        await _context.SaveChangesAsync();

        return new PaymentResponseDto
        {
            Id = payment.Id,
            TransactionReference = payment.TransactionReference,
            Status = payment.Status,
            Amount = payment.Amount,
            PaymentUrl = paymentUrl
        };
    }

    public async Task<Payment> UpdatePaymentStatus(string transactionReference, PaymentStatus status)
    {
        var payment = await _context.Payments.Include(p => p.Visit).FirstOrDefaultAsync(p => p.TransactionReference == transactionReference) ?? throw new ArgumentException($"No Payment Happend with Transation reference {transactionReference}");

        payment.Status = status;
        await _context.SaveChangesAsync();

        if (status == PaymentStatus.Success)
        {
            payment.Visit.Status = VisitStatus.Confirmed;
            await _context.SaveChangesAsync();
        }
        else if (status == PaymentStatus.Failed || status == PaymentStatus.Cancelled)
        {
            payment.Visit.Status = VisitStatus.Canceled;
            payment.Visit.CancellationReason = "Payment failed or cancelled";
            await _context.SaveChangesAsync();
        }
        return payment;
    }

    public async Task<bool> ProcessNursePayment(int visitId)
    {
        var visit = await _context.Visits.Include(v => v.Nurse).Include(v => v.Services).FirstOrDefaultAsync(v => v.Id == visitId) ?? throw new ArgumentException($"visit with Id: {visitId} not found");

        var payment = await _context.Payments.FirstOrDefaultAsync(p => p.VisitId == visitId) ?? throw new ArgumentException($"Payment for visit {visitId} not found");

        if (payment.NursePaid) return true;

        decimal nurseAmount = payment.NurseTotalProfit;

        payment.NursePaid = true;
        payment.NursePaymentDate = DateTime.Now;
        payment.NursePaymentReference = Guid.NewGuid().ToString();

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ProcessPaymentRefund(int visitId, string cancellationReason)
    {
        var visit = await _context.Visits.FirstOrDefaultAsync(v => v.Id == visitId) ?? throw new ArgumentException($"No visit with id {visitId} found");

        var payment = await _context.Payments.FirstOrDefaultAsync(p => p.VisitId == visitId) ?? throw new ArgumentException($"Payment for visit {visitId} not found");

        if (payment.Status != PaymentStatus.Success || visit.Status == VisitStatus.Done)
        {
            throw new Exception("This payment can not be refunded");
        }

        // create paymob refund request 
        var request = new HttpRequestMessage(HttpMethod.Post, "https://accept.paymob.com/api/acceptance/void_refund/refund");
        var apiKey = _config["PaymentGateways:Paymob:ApiKey"];
        request.Headers.Add("Authorization", $"Bearer {apiKey}");

        var refundRequest = new
        {
            auth_token = apiKey,
            transaction_id = payment.TransactionReference,
            amount_cents = (int)(payment.Amount * 100)
        };
        var jsonContent = JsonSerializer.Serialize(refundRequest);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        request.Content = content;

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        payment.Status = PaymentStatus.Refunded;
        visit.Status = VisitStatus.Canceled;
        visit.CancellationReason = cancellationReason;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<PaymentStatus> HandlePaymentWebhook(PaymentWebhookDto webhookData)
    {
        if (webhookData.Obj == null || webhookData.Obj.Order == null || webhookData.Obj.Order.Extras == null)
        {
            throw new ArgumentException("Invalid webhook data received");
        }

        var visitId = webhookData.Obj.Order.Extras.VisitId;
        var visit = await _context.Visits.FindAsync(visitId);
        
        if (visit == null)
        {
            throw new ArgumentException($"Visit with ID {visitId} not found");
        }

        var payment = await _context.Payments.FirstOrDefaultAsync(p => p.VisitId == visitId);
        
        if (payment == null)
        {
            throw new ArgumentException($"Payment for visit {visitId} not found");
        }

        var paymentStatus = PaymentStatus.Pending;

        if (webhookData.Obj.Success)
        {
            paymentStatus = PaymentStatus.Success;
            visit.Status = VisitStatus.Confirmed;
        }
        else if (webhookData.Obj.IsRefunded)
        {
            paymentStatus = PaymentStatus.Refunded;
            visit.Status = VisitStatus.Canceled;
            visit.CancellationReason = "Payment refunded";
        }
        else if (webhookData.Obj.IsVoided || !webhookData.Obj.Success)
        {
            paymentStatus = PaymentStatus.Failed;
            visit.Status = VisitStatus.Canceled;
            visit.CancellationReason = "Payment failed";
        }

        payment.Status = paymentStatus;
        await _context.SaveChangesAsync();

        return paymentStatus;
    }

    public class PaymentKeyResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Order_id { get; set; } = string.Empty;
    }
}