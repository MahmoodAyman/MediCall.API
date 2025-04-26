using Core.DTOs.Visit;
using Core.Interface;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VisitController : ControllerBase
    {
        private readonly IVisitService _visitService;

        public VisitController (IVisitService visitService){
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
            return Ok(nurses);
        }


    }
}
