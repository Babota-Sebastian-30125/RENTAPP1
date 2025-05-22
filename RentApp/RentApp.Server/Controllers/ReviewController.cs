using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentApp.Server.Models.DTO.Review;
using RentApp.Server.Service;
using System.Security.Claims;

namespace RentApp.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        // POST: api/reviews/{productId}
        [HttpPost("{productId}")]
        [Authorize]
        public async Task<IActionResult> AddReview(int productId, [FromBody] ReviewDTO dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized();

            try
            {
                var ok = await _reviewService.AddReviewAsync(productId, userId, dto);
                if (!ok)
                    return BadRequest(new { message = "Nu poti lasa review daca nu ai inchiriat produsul" });

                return Ok(new { message = "Review adaugat cu succes!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/reviews/product/{productId}
        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetReviewsForProduct(int productId)
        {
            var reviews = await _reviewService.GetReviewsForProductAsync(productId);
            return Ok(reviews);
        }

        // GET: api/reviews/product/{productId}/average
        [HttpGet("product/{productId}/average")]
        public async Task<IActionResult> GetAverageStars(int productId)
        {
            var avg = await _reviewService.GetAverageStarsAsync(productId);
            return Ok(new { average = avg });
        }
    }
}
