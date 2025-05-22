using RentApp.Server.Utils;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RentApp.Server.Models.DTO.Product
{
    public class ProductCreateWithFileDTO
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; } = string.Empty;

        public ProductCategory Category { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Range(0.01, 10000)]
        public decimal PricePerDay { get; set; }

        public bool Available { get; set; } = true;

        [AllowedExtensions(new[] { ".jpg", ".jpeg", ".png", ".gif" })]
        public IFormFile? ImageFile { get; set; }
        public Country Location { get; set; }

    }

}
