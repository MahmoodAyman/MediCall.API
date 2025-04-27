using API.SignalR;
using Core.DTOs.Payment;
using Core.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

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
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentDto request)
        {
            try
            {
                var payment = await _paymentService.CreateOrUpdatePayment(request.VisitId);
                return Ok(new
                {
                    payment.Id,
                    payment.TransactionReference,
                    payment.Status,
                    payment.Amount
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }


    }
}
