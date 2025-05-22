using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentApp.Server.Models;
using RentApp.Server.Models.DTO.Product;
using RentApp.Server.Models.DTO.Favorite;
using RentApp.Server.Service;
using System.Reflection;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
using IProductService = RentApp.Server.Models.DTO.Product.IProductService;
namespace RentApp.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        [Produces("application/json", "text/plain")]
        public async Task<ActionResult> GetProducts(
            [FromQuery] string search = "",
            ProductCategory? category = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            string sortBy = "price",
            Country? location = null,
            double? minRating = null)
        {
            var products = await _productService.GetProductsAsync(search, category, minPrice, maxPrice, sortBy, location, minRating);

            if (Request.Headers.Accept.Contains("text/plain"))
            {
                return Ok(string.Join("\n", products.Select(p => $"{p.Name} - {p.PricePerDay}")));
            }

            return Ok(new MessageResponse<IEnumerable<ProductDTO>>
            {
                Message = "Produse incarcate",
                Data = products
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);

            if (product == null)
            {
                return NotFound(new MessageResponse
                {
                    Message = $"Produsul cu ID-ul {id} nu a fost gasit"
                });
            }

            return Ok(new MessageResponse<ProductDTO>
            {
                Message = "Produs gasit cu succes.",
                Data = product
            });
        }

        [HttpPost]
        [Authorize]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> PostProduct([FromForm] ProductCreateWithFileDTO dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new MessageResponse
                {
                    Message = "Utilizatorul nu este autentificat"
                });
            }

            var userId = int.Parse(userIdClaim.Value);

            try
            {
                var product = await _productService.CreateProductAsync(dto, userId);

                return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, new MessageResponse<int>
                {
                    Message = "Produs creat cu succes",
                    Data = product.Id
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new MessageResponse
                {
                    Message = $"Eroare la crearea produsului: {ex.Message}"
                });
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] ProductCreateWithFileDTO dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new MessageResponse
                {
                    Message = "Utilizatorul nu este autentificat"
                });
            }

            var userId = int.Parse(userIdClaim.Value);

            try
            {
                var updated = await _productService.UpdateProductAsync(id, dto, userId);

                if (!updated)
                {
                    return Forbid("Nu aveti permisiunea de a edita acest produs");
                }

                return Ok(new MessageResponse
                {
                    Message = "Produs actualizat cu succes"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new MessageResponse
                {
                    Message = $"Eroare la actualizarea produsului: {ex.Message}"
                });
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new MessageResponse
                {
                    Message = "Utilizatorul nu este autentificat"
                });
            }

            var userId = int.Parse(userIdClaim.Value);

            var deleted = await _productService.DeleteProductAsync(id, userId);

            if (!deleted)
            {
                return Forbid("Nu aveti permisiunea de a sterge acest produs");
            }

            return Ok(new MessageResponse
            {
                Message = "Produs sters cu succes"
            });
        }

        [HttpGet("categories")]
        public ActionResult<IEnumerable<string>> GetCategories()
        {
            var categories = Enum.GetValues(typeof(ProductCategory))
                .Cast<ProductCategory>()
                .Select(c =>
                {
                    var display = c.GetType()
                                   .GetMember(c.ToString())
                                   .First()
                                   .GetCustomAttribute<DisplayAttribute>()?
                                   .Name ?? c.ToString();

                    return display;
                });

            return Ok(categories);
        }

        [HttpGet("locations")]
        public ActionResult<IEnumerable<string>> GetLocations()
        {
            var locations = Enum.GetValues(typeof(Country))
                .Cast<Country>()
                .Select(c => c.ToString());

            return Ok(locations);
        }
    }
}