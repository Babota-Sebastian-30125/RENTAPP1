using Humanizer;
using Microsoft.EntityFrameworkCore;
using RentApp.Data;
using RentApp.Models;
using RentApp.Server.Models;
using RentApp.Server.Models.DTO.Product;
using System.Diagnostics.Metrics;

namespace RentApp.Server.Service
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTO>> GetProductsAsync(string search, ProductCategory? category, decimal? minPrice, decimal? maxPrice, string sortBy, Country? location, double? minRating);

        Task<ProductDetailsDTO?> GetProductByIdAsync(int id);
        Task<Product> CreateProductAsync(ProductCreateWithFileDTO dto, int userId);
        Task<bool> UpdateProductAsync(int id, ProductCreateWithFileDTO dto, int userId);
        Task<bool> DeleteProductAsync(int id, int userId);
    }

    public class ProductService : IProductService
    {
        private readonly RentDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductService(RentDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IEnumerable<ProductDTO>> GetProductsAsync(string search, ProductCategory? category, decimal? minPrice, decimal? maxPrice, string sortBy, Country? location, double? minRating)
        {
            var query = _context.Products
          .Include(p => p.user)
          .Include(p => p.Reviews) 
         .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var loweredSearch = search.ToLower();
                query = query.Where(p =>
                    p.Name.ToLower().Contains(loweredSearch) ||
                    p.Description.ToLower().Contains(loweredSearch));
            }

            if (category.HasValue)
            {
                var categoryStr = category.Value.ToString(); 
                query = query.Where(p => p.Category == categoryStr);
            }

            if (location.HasValue)
            {
                var locationStr = location.Value.ToString();
                query = query.Where(p => p.Location == locationStr);
            }


            if (minPrice.HasValue)
                query = query.Where(p => p.PricePerDay >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => p.PricePerDay <= maxPrice.Value);

            if (minRating.HasValue)
                query = query.Where(p => p.Reviews.Any() && p.Reviews.Average(r => r.Stars) >= minRating.Value);

            query = sortBy?.ToLower() switch
            {
                "price" => query.OrderBy(p => p.PricePerDay),
                "newest" => query.OrderByDescending(p => p.AddedAt),
                "rating" => query.OrderByDescending(p => p.Reviews.Any() ? p.Reviews.Average(r => r.Stars) : 0),
                _ => query.OrderBy(p => p.Name),
            };

            return await query.Select(p => new ProductDTO
            {
                Id = p.Id,
                Category = p.Category.ToString(),
                Name = p.Name,
                Description = p.Description,
                AddedAt = p.AddedAt,
                Available = p.Available,
                PricePerDay = p.PricePerDay,
                UserName = p.user.Name,
                ImagePath = p.ImagePath != null ? $"https://localhost:7020{p.ImagePath}" : null,
                Location = p.Location.ToString(),
                AverageRating = p.Reviews.Any() ? p.Reviews.Average(r => r.Stars) : (double?)null
            }).ToListAsync();
        }

        public async Task<ProductDetailsDTO?> GetProductByIdAsync(int id)
        {
            return await _context.Products.Include(p => p.user)
                .Where(p => p.Id == id)
                .Select(p => new ProductDetailsDTO
                {
                    Id = p.Id,
                    Category = p.Category,
                    Name = p.Name,
                    Description = p.Description,
                    AddedAt = p.AddedAt,
                    Available = p.Available,
                    PricePerDay = p.PricePerDay,
                    UserName = p.user.Name,
                    TelephoneNumber = p.user.telephoneNumber,
                    ImagePath = p.ImagePath != null ? $"https://localhost:7020{p.ImagePath}" : null,
                    Location = p.Location
                }).FirstOrDefaultAsync();
        }

        public async Task<Product> CreateProductAsync(ProductCreateWithFileDTO dto, int userId)
        {

           

            var product = new Product
            {
                Name = dto.Name,
                Category = dto.Category.ToString(),
                Description = dto.Description,
                PricePerDay = dto.PricePerDay,
                Available = dto.Available,
                AddedAt = DateTime.UtcNow,
                UserId = userId,
                Location = dto.Location.ToString()
            };


            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                var ext = Path.GetExtension(dto.ImageFile.FileName).ToLower();
                var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif" };

                if (!allowed.Contains(ext))
                    throw new Exception("Fisierul nu este un tip de imagine valid");

                var folder = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(folder);

                var fileName = Guid.NewGuid() + ext;
                var path = Path.Combine(folder, fileName);

                using var stream = new FileStream(path, FileMode.Create);
                await dto.ImageFile.CopyToAsync(stream);

                product.ImagePath = "/uploads/" + fileName;
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<bool> UpdateProductAsync(int id, ProductCreateWithFileDTO dto, int userId)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null || product.UserId != userId) return false;

            product.Name = dto.Name;
            product.Category = dto.Category.ToString();
            product.Description = dto.Description;
            product.Available = dto.Available;
            product.PricePerDay = dto.PricePerDay;
            product.Location = dto.Location.ToString();

            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                var ext = Path.GetExtension(dto.ImageFile.FileName).ToLower();
                var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif" };

                if (!allowed.Contains(ext))
                    throw new Exception("Fisierul nu este un tip de imagine valid");

                var folder = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(folder);

                var fileName = Guid.NewGuid() + ext;
                var path = Path.Combine(folder, fileName);

                using var stream = new FileStream(path, FileMode.Create);
                await dto.ImageFile.CopyToAsync(stream);

                product.ImagePath = "/uploads/" + fileName;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteProductAsync(int id, int userId)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null || product.UserId != userId) return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}