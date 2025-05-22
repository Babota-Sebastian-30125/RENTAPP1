using Microsoft.EntityFrameworkCore;
using RentApp.Data;
using RentApp.Models;
using RentApp.Server.Models;
using RentApp.Server.Models.DTO.Product;

namespace RentApp.Server.Service
{
    public interface IFavoriteService
    {
        Task<bool> ToggleFavoriteAsync(int userId, int productId);
        Task<List<Product>> GetUserFavoritesAsync(int userId);
        Task<bool> RemoveFavoriteAsync(int userId, int productId);
    }
    public class FavoriteService : IFavoriteService
    {
        private readonly RentDbContext _context;

        public FavoriteService(RentDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ToggleFavoriteAsync(int userId, int productId)
        {
            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.ProductId == productId);

            if (favorite != null)
            {
                
                _context.Favorites.Remove(favorite);
                await _context.SaveChangesAsync();
                return false; 
            }
            else
            {
               
                var newFavorite = new Favorite { UserId = userId, ProductId = productId };
                _context.Favorites.Add(newFavorite);
                await _context.SaveChangesAsync();
                return true; 
            }
        }

        public async Task<List<Product>> GetUserFavoritesAsync(int userId)
        {
            return await _context.Favorites
                .Where(f => f.UserId == userId)
                .Select(f => f.Product)
                .ToListAsync();
        }
        public async Task<bool> RemoveFavoriteAsync(int userId, int productId)
        {
            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.ProductId == productId);

            if (favorite == null)
                return false;

            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
