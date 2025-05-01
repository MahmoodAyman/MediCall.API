using Core.DTOs;
using Core.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize(Roles = "Patient")]
    [Route("api/[controller]")]
    [ApiController]
    public class PatientIllnessController (IPatientIllnessesService patientIllnessesService): ControllerBase
    {
        private readonly IPatientIllnessesService _patientIllnessesService = patientIllnessesService;
        
        
        [HttpGet]
        public async Task<IActionResult> GetAllIllnesses()
        {
            var uid = User.FindFirst("uid")?.Value;
            if (uid == null)
            {
                return BadRequest("User ID not found.");
            }
            var illnesses = await _patientIllnessesService.GetAllPatientIllnessesForUserAsync(uid);
            return Ok(illnesses);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetIllnessById(int id)
        {
            var uid = User.FindFirst("uid")?.Value;
            if (uid == null)
            {
                return BadRequest("User ID not found.");
            }
            try
            {
                var illness = await _patientIllnessesService.GetPatientIllnessByIdAsync(id, uid);
                if (illness == null)
                {
                    return NotFound();
                }
                return Ok(illness);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddIllness([FromBody] PatientIllnessesDTO patientIllness)
        {
            var uid = User.FindFirst("uid")?.Value;
            if (uid == null)
            {
                return BadRequest("User ID not found.");
            }
            patientIllness.PatientId = uid;
            try
            {
                var addedIllness = await _patientIllnessesService.AddPatientIllnessAsync(patientIllness);
                return CreatedAtAction(nameof(GetIllnessById), new { id = addedIllness.PatientId , illnessId =addedIllness.IllnessId }, addedIllness);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateIllness([FromBody] PatientIllnessesDTO patientIllness)
        {
            var uid = User.FindFirst("uid")?.Value;
            if (uid == null)
            {
                return BadRequest("User ID not found.");
            }
            patientIllness.PatientId = uid;
            try
            {
                var updatedIllness = await _patientIllnessesService.UpdatePatientIllnessAsync(patientIllness);
                return Ok(updatedIllness);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIllness(int id)
        {
            var uid = User.FindFirst("uid")?.Value;
            if (uid == null)
            {
                return BadRequest("User ID not found.");
            }
            try
            {
                await _patientIllnessesService.DeletePatientIllnessAsync(id, uid);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }
    }
}
