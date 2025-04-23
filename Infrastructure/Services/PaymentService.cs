using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Enums;
using Core.Interface;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Infrastructure.Data;

namespace Infrastructure.Services;

public class PaymentService(IConfiguration _config, MediCallContext _context, HttpClient _httpClient) : IPaymentService
{
    public async Task<Payment> CreateOrUpdatePayment(int visitId)
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
            return existingPayment;
        }

        // Create new Payment request
        var request = new HttpRequestMessage(HttpMethod.Post, "https://accept.paymob.com/v1/intention/");

        var apiKey = _config["PaymentGateways:Paymob:SecretKey"];

        request.Headers.Add("Authorization", $"Token {apiKey}");

        decimal totalAmount = visit.CalculateTotalCost();

        // payment request payload: 
        var paymentRequest = new
        {
            amount = (int)totalAmount,
            currency = "EGP",
            payment_methods = new object[] { 12, "card", _config["PaymentGateways:Paymob:PaymentIntegrationId"]!, },
            items = new[] {
                new {
                    name = "Healthcare visit",
                    amount = (int) totalAmount,
                    description = $"Healthcare visit on {visit.ScheduledDate:yyyy-MM-dd}",
                    quantity = 1
                }
            },
            billing_data = new
            {
                appartment = "N/A",
                first_name = visit.Patient.FirstName,
                last_name = visit.Patient.LastName,
                street = $"Coordinates : {visit.PatientLocation.Lat}, {visit.PatientLocation.Lng}",
                building = "N/A",
                phone_number = visit.Patient.PhoneNumber,
                country = "EGY",
                email = visit.Patient.Email,
                floor = "N/A",
                state = "Cairo",
            },
            customer = new
            {
                first_name = visit.Patient.FirstName,
                last_name = visit.Patient.LastName,
                email = visit.Patient.Email,
            },
            extras = new
            {
                visit_id = visit.Id
            }
        };

        var jsonContent = JsonSerializer.Serialize(paymentRequest);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var responseData = JsonSerializer.Deserialize<PaymobResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });


        var payment = new Payment
        {
            PaymentMethod = PaymentMethod.CreditCard,
            PaymentDate = DateTime.Now,
            Status = PaymentStatus.Pending,
            TransactionReference = responseData?.Id ?? Guid.NewGuid().ToString(),
            VisitId = visit.Id,
            Visit = visit,
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();

        visit.Status = VisitStatus.Confirmed;
        await _context.SaveChangesAsync();
        return payment;

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


    public class PaymobResponse
    {
        public Guid Id { get; set; }
        public string RedirectURL { get; set; }
    }
}