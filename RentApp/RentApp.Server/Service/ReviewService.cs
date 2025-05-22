using RentApp.Data;
using RentApp.Server.Models.DTO.Review;
using RentApp.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace RentApp.Server.Service
{
    public interface IReviewService
    {
        Task<bool> AddReviewAsync(int productId, int userId, ReviewDTO dto);
        Task<double> GetAverageStarsAsync(int productId);
        Task<List<ReviewDTO>> GetReviewsForProductAsync(int productId);
    }

    public class ReviewService : IReviewService
    {
        private readonly RentDbContext _context;

        public ReviewService(RentDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddReviewAsync(int productId, int userId, ReviewDTO dto)
        {
            // userul a inchiriat produsul 
            var hasRented = await _context.Rentals.AnyAsync(r => r.ProductId == productId && r.UserId == userId);
            if (!hasRented)
                return false;

            // userul a dat deja review la produs 
            var alreadyReviewed = await _context.Reviews.AnyAsync(r => r.ProductId == productId && r.UserId == userId);
            if (alreadyReviewed)
                throw new Exception("Ai lasat deja un review la acest produs.");

            var review = new Review
            {
                ProductId = productId,
                UserId = userId,
                Stars = dto.Stars,
                Comment = dto.Comment,
                Date = DateTime.UtcNow
            };
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<double> GetAverageStarsAsync(int productId)
        {
            var stars = await _context.Reviews
                .Where(r => r.ProductId == productId)
                .Select(r => r.Stars)
                .ToListAsync();

            return stars.Any() ? stars.Average() : 0.0;
        }
        public async Task<List<ReviewDTO>> GetReviewsForProductAsync(int productId)
        {
            return await _context.Reviews
                .Where(r => r.ProductId == productId)
                .OrderByDescending(r => r.Date)
                .Select(r => new ReviewDTO
                {
                    Stars = r.Stars,
                    Comment = r.Comment
                })
                .ToListAsync();
        }
    }
}
