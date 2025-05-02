using Core.DTOs;
using Core.Interface;
using Core.Models;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewingController(IReviewingService reviewingService,MediCallContext mediCall) : ControllerBase
    {
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateReview([FromBody] ReviewingDTO dto)
        {
            var userId = User.FindFirst("id").Value; 

            var visit = await mediCall.Visits.FirstOrDefaultAsync(v => v.Id == dto.VisitId && v.PatientId == userId);

            if (visit == null)
            {
                return BadRequest(new { message = "Invalid visit or not authorized." });
            }

            if (await reviewingService.IsVisitAlreadyReviewedAsync(dto.VisitId))
            {
                return BadRequest(new { message = "Visit already reviewed." });
            }

            var review = new Reviewing
            {
                VisitId = dto.VisitId,
                Rating = dto.Rating,
                Comment = dto.Comment
            };

            await reviewingService.AddReviewAsync(review);

            return Ok(new { message = "Review submitted successfully." });
        }

        [HttpGet("nurse/{nurseId}")]
        public async Task<IActionResult> GetNurseReviews(string nurseId)
        {
            var reviews = await reviewingService.GetReviewsByNurseIdAsync(nurseId);
            return Ok(reviews);
        }

        [HttpGet("nurse/{nurseId}/average")]
        public async Task<IActionResult> GetNurseAverageRating(string nurseId)
        {
            var average = await reviewingService.GetAverageRatingForNurseAsync(nurseId);
            return Ok(new { averageRating = average });
        }
    }
}
