using Core.DTOs.Payment;
using Core.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController(IPaymentService _paymentService) : ControllerBase
    {
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
