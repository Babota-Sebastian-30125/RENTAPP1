using System.Collections.Generic;
using System.Threading.Tasks;

namespace RentApp.Server.Models.DTO.Product
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTO>> GetProductsAsync(string search = "", ProductCategory? category = null,
            decimal? minPrice = null, decimal? maxPrice = null, string sortBy = "price",
            Country? location = null, double? minRating = null);

        Task<ProductDTO?> GetProductByIdAsync(int productId);

        Task<ProductDTO> CreateProductAsync(ProductCreateWithFileDTO dto, int userId);

        Task<bool> UpdateProductAsync(int productId, ProductCreateWithFileDTO dto, int userId);

        Task<bool> DeleteProductAsync(int productId, int userId);
    }
}
