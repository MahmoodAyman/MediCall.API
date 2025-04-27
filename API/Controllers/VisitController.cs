using API.SignalR;
using Core.DTOs.Visit;
using Core.Interface;
using Infrastructure.Data;
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
        [HttpPost("find-nurse")]
        public async Task<IActionResult> FindNearestNurses(CreateVisitDto visit)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (string.IsNullOrEmpty(visit.PatientId))
            {
                return BadRequest("Visit Id not provided");
            }
            var nurses = await _visitService.GetNearNurses(visit);
            var createPendingVsist = _visitService.CreatePendingVisitAsync(visit);

            var hubContext = HttpContext.RequestServices.GetRequiredService<IHubContext<VisitHub>>();

            foreach (var nurse in nurses)
            {
                await hubContext.Clients.User(nurse.Id).SendAsync("New Visit Request", createPendingVsist, visit);
            }

            return Ok(nurses);
        }

        [HttpPost("accept-visit")]
        public async Task<IActionResult> AcceptVisit(int visitId, string nurseId)
        {
            if (string.IsNullOrWhiteSpace(nurseId))
            {
                return BadRequest("No Nurse ID provided");
            }
            var result = await _visitService.AcceptVisitByNurse(visitId, nurseId);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);
        }
    }
}
