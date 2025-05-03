using Infrastructure.SignalR;
using Core.DTOs.Payment;
using Core.Enums;
using Core.Interface;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IHubContext<VisitHub> _hubContext;
        public PaymentController(IPaymentService paymentService, IHubContext<VisitHub> hubContext)
        {
            _paymentService = paymentService;
            _hubContext = hubContext;
        }

        [HttpPost]
        // [Authorize(Roles = "Patient")]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentDto request)
        {
            try
            {
                var paymentResponse = await _paymentService.CreateOrUpdatePayment(request.VisitId);
                return Ok(paymentResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("{visitId}")]
        // [Authorize(Roles = "Patient,Nurse")]
        public async Task<IActionResult> GetPaymentDetails(int visitId)
        {
            try
            {
                var paymentResponse = await _paymentService.CreateOrUpdatePayment(visitId);
                return Ok(paymentResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("webhook")]
        [AllowAnonymous]
        public async Task<IActionResult> PaymentWebhook()
        {
            try
            {
                using var reader = new StreamReader(Request.Body);
                var body = await reader.ReadToEndAsync();
                
                var webhookData = JsonSerializer.Deserialize<PaymentWebhookDto>(body, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });

                if (webhookData == null || webhookData.Obj == null)
                {
                    return BadRequest(new { error = "Invalid webhook data" });
                }

                var status = await _paymentService.HandlePaymentWebhook(webhookData);
                
                if (webhookData.Obj.Order?.Extras?.VisitId > 0)
                {
                    int visitId = webhookData.Obj.Order.Extras.VisitId;
                    await _hubContext.Clients.Group($"visit_{visitId}").SendAsync("PaymentStatusChanged", new
                    {
                        VisitId = visitId,
                        Status = status.ToString()
                    });
                }

                return Ok(new { message = "Payment status updated" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("process-nurse-payment/{visitId}")]
        // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ProcessNursePayment(int visitId)
        {
            try
            {
                var result = await _paymentService.ProcessNursePayment(visitId);
                return Ok(new { success = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("refund/{visitId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RefundPayment(int visitId, [FromBody] RefundRequest request)
        {
            try
            {
                var result = await _paymentService.ProcessPaymentRefund(visitId, request.CancellationReason);
                return Ok(new { success = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }

    public class RefundRequest
    {
        public string CancellationReason { get; set; } = "Refunded by admin";
    }
}
