using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentApp.Data;
using RentApp.Models;
using RentApp.Server.Models;
using RentApp.Server.Models.DTO.User;
using System.Security.Claims;

namespace RentApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly RentDbContext _context;

        public UserController(RentDbContext context)
        {
            _context = context;
        }

        // GET: api/user/profile
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized(new MessageResponse{ Error = "Utilizator neautorizat" });

            var user = await _context.Users
                .Where(u => u.UserId == userId)
                .Select(u => new
                {
                    u.UserId,
                    u.Name,
                    u.email,
                    u.telephoneNumber
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound(new MessageResponse { Error = "Utilizatorul nu a fost gasit" });

            return Ok(user);
        }

        // PUT: api/user/profile
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateAccountDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            if (userId == null)
                return Unauthorized(new MessageResponse{ Error = "Neautorizat" });

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound(new MessageResponse { Error = "Utilizatorul nu a fost gasit" });

            user.Name = dto.Name ?? user.Name;
            user.email = dto.Email ?? user.email;
            user.telephoneNumber = dto.TelephoneNumber ?? user.telephoneNumber;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok(new MessageResponse { Message = "Contul a fost actualizat cu succes" });
        }

        // PUT: api/user/change-password
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            if (userId == null)
                return Unauthorized(new MessageResponse { Error = "Neautorizat" });

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound(new MessageResponse{ Error = "Utilizatorul nu a fost gasit" });

            if (!BCrypt.Net.BCrypt.Verify(dto.OldPassword, user.password))
                return BadRequest(new MessageResponse { Error = "Parola veche este incorecta" });

            user.password = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            await _context.SaveChangesAsync();

            return Ok(new MessageResponse{ Message = "Parola a fost schimbata cu succes" });
        }

        private int? GetUserId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return userId != null ? int.Parse(userId) : (int?)null;
        }
        [Authorize]
        [HttpDelete("delete-account")]
        public async Task<IActionResult> DeleteAccount()
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized(new MessageResponse { Error = "Neautorizat" });

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var user = await _context.Users
                    .Include(u => u.Rentals)          // Rentals ale user-ului
                    .Include(u => u.Products)         // Produsele user-ului
                        .ThenInclude(p => p.Rentals)  // Rentals ale produselor
                    .Include(u => u.Products)
                        .ThenInclude(p => p.Reviews)  // Reviews ale produselor
                    .Include(u => u.Rentals)
                    .ThenInclude(r => r.product)
                    .FirstOrDefaultAsync(u => u.UserId == userId);

                if (user == null)
                    return NotFound(new MessageResponse { Error = "Utilizatorul nu a fost gasit" });

                //  Reviews scrise de user
                var reviewsWrittenByUser = await _context.Reviews.Where(r => r.UserId == userId).ToListAsync();
                _context.Reviews.RemoveRange(reviewsWrittenByUser);

                // Favorite legate de user
                var favoritesByUser = await _context.Set<Favorite>().Where(f => f.UserId == userId).ToListAsync();
                _context.Set<Favorite>().RemoveRange(favoritesByUser);

                //  Favorites legate de produsele user-ului
                var productIds = user.Products.Select(p => p.Id).ToList();
                var favoritesForProducts = await _context.Set<Favorite>().Where(f => productIds.Contains(f.ProductId)).ToListAsync();
                _context.Set<Favorite>().RemoveRange(favoritesForProducts);

                // Reviews pentru produsele user-ului
                var reviewsForProducts = user.Products.SelectMany(p => p.Reviews ?? new List<Review>()).ToList();
                _context.Reviews.RemoveRange(reviewsForProducts);

                //  Rentals legate de produse user-ului
                var rentalsForProducts = user.Products.SelectMany(p => p.Rentals ?? new List<Rental>()).ToList();
                _context.Rentals.RemoveRange(rentalsForProducts);

                // Rentals ale user-ului 
                _context.Rentals.RemoveRange(user.Rentals);

                // produsele user-ului
                _context.Products.RemoveRange(user.Products);


                _context.Users.Remove(user);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new MessageResponse{ Message = "Contul a fost sters cu succes" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                Console.WriteLine("Eroare la stergere cont:");
                Exception inner = ex;
                while (inner != null)
                {
                    Console.WriteLine(inner.Message);
                    inner = inner.InnerException;
                }

                return StatusCode(500, new MessageResponse { Error = "Eroare la stergerea contului", Details = ex.Message });
            }
        }
    }
}

