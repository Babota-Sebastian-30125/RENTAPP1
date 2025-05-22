using Microsoft.EntityFrameworkCore;
using RentApp.Data;
using RentApp.Models;
using RentApp.Server.Models;
using RentApp.Server.Models.DTO.Rental;

namespace RentApp.Server.Service
{
    public interface IRentalService
    {
        Task<int> RentProductAsync(RentalRequestDTO dto, int userId);
        Task<IEnumerable<RentalListItemDTO>> GetMyRentalsAsync(int userId);
        Task<bool> CancelRentalAsync(int rentalId, int userId);
        Task<ProductRentalDetailsDTO?> GetProductDetailsAsync(int productId);
    }
    public class RentalService : IRentalService
    {
        private readonly RentDbContext _context;

        public RentalService(RentDbContext context)
        {
            _context = context;
        }

        public async Task<int> RentProductAsync(RentalRequestDTO dto, int userId)
        {
            var product = await _context.Products.FindAsync(dto.ProductId);
            if (product == null || !product.Available)
                throw new Exception("Produsul nu este disponibil pentru inchiriere");

            var days = (dto.EndDate - dto.StartDate).Days;
            if (days <= 0)
                throw new Exception("Perioada de inchiriere este invalida");

            var rental = new Rental
            {
                ProductId = dto.ProductId,
                UserId = userId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                RegisterDate = DateTime.UtcNow,
                TotalPrice = product.PricePerDay * days,
                Status = States.Confirmed
            };

            product.Available = false;
            _context.Rentals.Add(rental);
            await _context.SaveChangesAsync();

            return rental.RentalId ?? 0;
        }

        public async Task<IEnumerable<RentalListItemDTO>> GetMyRentalsAsync(int userId)
        {
            var rentals = await _context.Rentals
                .Include(r => r.product)
                .Where(r => r.UserId == userId)
                .ToListAsync();

            return rentals.Select(r => new RentalListItemDTO
            {
                RentalId = r.RentalId ?? 0,
                ProductName = r.product.Name,
                StartDate = r.StartDate,
                EndDate = r.EndDate,
                TotalPrice = r.TotalPrice,
                Status = r.Status.ToString() 
            });
        }

        public async Task<bool> CancelRentalAsync(int rentalId, int userId)
        {
            var rental = await _context.Rentals
                .Include(r => r.product)
                .FirstOrDefaultAsync(r => r.RentalId == rentalId && r.UserId == userId);

            if (rental == null)
                return false;

            if (rental.Status != States.Confirmed)
                throw new Exception("Doar inchirierile confirmate pot fi anulate");

            rental.Status = States.Cancelled;
            rental.product.Available = true;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ProductRentalDetailsDTO?> GetProductDetailsAsync(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                return null;

            return new ProductRentalDetailsDTO
            {
                Id = product.Id,
                Name = product.Name,
                PricePerDay = product.PricePerDay,
                Description = product.Description,
                Available = product.Available
            };
        }
    }
}