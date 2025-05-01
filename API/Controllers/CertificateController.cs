using System.Security.Claims;
using Core.DTOs;
using Core.Interface;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize("Nurse")]
    public class CertificateController(INurseCertificateService _nurseCertificateService) : ControllerBase
    {
        [HttpGet("GetNurseCertificates")]
        public async Task<ActionResult<IEnumerable<NurseCertificate>>> GetNurseCertificates()
        {
            var nurseId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var nurseCertificates = await _nurseCertificateService.GetAllNurseCertificatesAsync(nurseId!);
            if (nurseCertificates == null) return NotFound();
            return Ok(nurseCertificates);
        }
        [HttpGet("{certificateById}")]
        public async Task<ActionResult<NurseCertificate>> GetNurseCertificate(string nurseId, int certificateId)
        {
            var nurseCertificate = await _nurseCertificateService.GetNurseCertificateAsync(nurseId, certificateId);
            if (nurseCertificate == null) return NotFound();
            return Ok(nurseCertificate);
        }
        [HttpPost]
        public async Task<ActionResult<NurseCertificate>> AddNurseCertificate([FromForm] NurseCertificateDTO certificateDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var nurseId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (nurseId == null) return NotFound();
            var certificate = await _nurseCertificateService.AddCertificateAsync(nurseId, certificateDto);
            return CreatedAtAction(nameof(GetNurseCertificate), new { certificateId = certificate.CertificateId }, certificate);

        }
        [HttpPut("certificateId")]
        public async Task<ActionResult<NurseCertificate>> UpdateNurseCertificate(int certificateId, [FromForm] NurseCertificateDTO certificateDto)
        {

            if (!ModelState.IsValid) return BadRequest(ModelState);
            var nurseId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var certificate = await _nurseCertificateService.UpdateCertificateAsync(nurseId!, certificateId, certificateDto);
            if (certificate == null) return NotFound();
            return Ok(certificate);
        }

        [HttpDelete("{certificateId}")]
        public async Task<ActionResult> DeleteCertificate(int certificateId)
        {
            var nurseId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _nurseCertificateService.DeleteCertificatedAsync(nurseId!, certificateId);
            if (!result) return NotFound();
            return NoContent();
        }

    }
}
