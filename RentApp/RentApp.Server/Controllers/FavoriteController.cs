using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentApp.Models;
using RentApp.Server.Models.DTO.Favorite;
using RentApp.Server.Service;
using System.Security.Claims;

namespace RentApp.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FavoritesController : ControllerBase
    {
        private readonly IFavoriteService _favoriteService;

        public FavoritesController(IFavoriteService favoriteService)
        {
            _favoriteService = favoriteService;
        }

        [HttpPost("{productId}")]
        public async Task<IActionResult> ToggleFavorite(int productId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var added = await _favoriteService.ToggleFavoriteAsync(userId, productId);
            return Ok(new MessageResponse { Message = added ? "Adaugat la favorite" : "Sters din favorite" });
        }

        [HttpGet]
        public async Task<IActionResult> GetFavorites()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var favorites = await _favoriteService.GetUserFavoritesAsync(userId);
            return Ok(favorites);
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> RemoveFavorite(int productId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var removed = await _favoriteService.RemoveFavoriteAsync(userId, productId);
            if (removed)
                return Ok(new MessageResponse { Message = "Produsul a fost scos din favorite" });
            else
                return NotFound(new MessageResponse{ Message = "Produsul nu a fost gasit in favorite" });
        }
    }
}
