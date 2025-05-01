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
            var userId = User.FindFirst("UserId")?.Value;
            if (userId == null)
            {
                return BadRequest("User ID not found.");
            }
            var illnesses = await _patientIllnessesService.GetAllPatientIllnessesForUserAsync(userId);
            return Ok(illnesses);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetIllnessById(int id)
        {
            var userId = User.FindFirst("UserId")?.Value;
            if (userId == null)
            {
                return BadRequest("User ID not found.");
            }
            try
            {
                var illness = await _patientIllnessesService.GetPatientIllnessByIdAsync(id, userId);
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
            var userId = User.FindFirst("UserId")?.Value;
            if (userId == null)
            {
                return BadRequest("User ID not found.");
            }
            patientIllness.PatientId = userId;
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
            var userId = User.FindFirst("UserId")?.Value;
            if (userId == null)
            {
                return BadRequest("User ID not found.");
            }
            patientIllness.PatientId = userId;
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
            var userId = User.FindFirst("UserId")?.Value;
            if (userId == null)
            {
                return BadRequest("User ID not found.");
            }
            try
            {
                await _patientIllnessesService.DeletePatientIllnessAsync(id, userId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }
    }
}
