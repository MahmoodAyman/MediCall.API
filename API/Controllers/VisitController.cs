using Infrastructure.SignalR;
using Core.DTOs.Payment;
using Core.DTOs.Visit;
using Core.Enums;
using Core.Interface;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VisitController : ControllerBase
    {
        private readonly IVisitService _visitService;
        private readonly IPaymentService _paymentService;

        public VisitController(IVisitService visitService, IPaymentService paymentService)
        {
            _visitService = visitService;
            _paymentService = paymentService;
        }
        [Authorize(Roles = "Patient")]
        [HttpPost("find-nurse")]
        public async Task<IActionResult> FindNearestNurses(RequestNearNursesDTO requestNeerNursesDTO)
        {
            var patientId = User.FindFirst("uid")?.Value;
            if (string.IsNullOrWhiteSpace(patientId))
            {
                return BadRequest("No Patient ID provided");
            }
            requestNeerNursesDTO.PatientId = patientId;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var responseNeerNursesDTO = await _visitService.GetNearNurses(requestNeerNursesDTO);
            if(!responseNeerNursesDTO.Success)
            {
                return BadRequest(responseNeerNursesDTO.Message);
            }

            return Ok(responseNeerNursesDTO);
        }

        [Authorize(Roles = "Nurse")]
        [HttpPost("accept-visit-by-nurse")]
        public async Task<IActionResult> AcceptVisit(int visitId)
        {
            var nurseId = User.FindFirst("uid")?.Value;
            if (string.IsNullOrWhiteSpace(nurseId))
            {
                return BadRequest("No Nurse ID provided");
            }
            if (visitId <= 0)
            {
                return BadRequest("No Visit ID provided");
            }
            var result = await _visitService.AcceptVisitByNurse(visitId, nurseId);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);
        }

        [Authorize(Roles = "Patient")]
        [HttpPost("accept-nurse-by-patient")]
        public async Task<IActionResult> AcceptNurse(int visitId, string nurseId)
        {
            var patientId = User.FindFirst("uid")?.Value;
            if (string.IsNullOrWhiteSpace(nurseId))
            {
                return BadRequest("No Nurse ID provided");
            }
            if (visitId <= 0)
            {
                return BadRequest("No Visit ID provided");
            }
            var result = await _visitService.AcceptNurseByPatient(visitId, nurseId, patientId);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            // Get payment information
            try
            {
                var paymentResponse = await _paymentService.CreateOrUpdatePayment(visitId);
                return Ok(new 
                { 
                    result.Success, 
                    result.Message, 
                    result.Visit, 
                    result.Nurses,
                    Payment = paymentResponse
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    result.Success,
                    result.Message,
                    result.Visit,
                    result.Nurses,
                    PaymentError = ex.Message
                });
            }
        }

        [Authorize(Roles = "Patient")]
        [HttpPost("cancel-visit-by-patient")]
        public async Task<IActionResult> CancelVisitByPatient(int visitId, string canclationReson)
        {
            var patientId = User.FindFirst("uid")?.Value;
            if (string.IsNullOrWhiteSpace(patientId))
            {
                return BadRequest("No Patient ID provided");
            }
            if (visitId <= 0)
            {
                return BadRequest("No Visit ID provided");
            }
            var result = await _visitService.CancelVisitByPatient(visitId, patientId, canclationReson);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);
        }

        [Authorize(Roles = "Nurse")]
        [HttpPost("cancel-visit-by-nurse")]
        public async Task<IActionResult> CancelVisitByNurse(int visitId, string canclationReson)
        {
            var nurseId = User.FindFirst("uid")?.Value;
            if (string.IsNullOrWhiteSpace(nurseId))
            {
                return BadRequest("No Nurse ID provided");
            }
            if (visitId <= 0)
            {
                return BadRequest("No Visit ID provided");
            }
            var result = await _visitService.CancelVisitByNurse(visitId, nurseId, canclationReson);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);
        }

        [Authorize(Roles = "Patient")]
        [HttpPost("complete-visit-by-patient")]
        public async Task<IActionResult> CompleteVisitByPatient(int visitId)
        {
            var patientId = User.FindFirst("uid")?.Value;
            if (string.IsNullOrWhiteSpace(patientId))
            {
                return BadRequest("No Patient ID provided");
            }
            if (visitId <= 0)
            {
                return BadRequest("No Visit ID provided");
            }
            var result = await _visitService.CompleteVisitByPatient(visitId, patientId);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);
        }

    }
}
