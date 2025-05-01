using API.SignalR;
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

        public VisitController(IVisitService visitService)
        {
            _visitService = visitService;
        }
        [Authorize(Roles = "Patient")]
        [HttpPost("find-nurse")]
        public async Task<IActionResult> FindNearestNurses(RequestNearNursesDTO requestNeerNursesDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var responseNeerNursesDTO = await _visitService.GetNearNurses(requestNeerNursesDTO);
            if(!responseNeerNursesDTO.Success)
            {
                return BadRequest(responseNeerNursesDTO.Message);
            }

            return Ok(new {responseNeerNursesDTO.Nurses});
        }

        [Authorize(Roles = "Nurse")]
        [HttpPost("accept-visit-by-nurse")]
        public async Task<IActionResult> AcceptVisit(int visitId, string nurseId)
        {
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
            if (string.IsNullOrWhiteSpace(nurseId))
            {
                return BadRequest("No Nurse ID provided");
            }
            if (visitId <= 0)
            {
                return BadRequest("No Visit ID provided");
            }
            var result = await _visitService.AcceptNurseByPatient(visitId, nurseId);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);
        }
    }
}
