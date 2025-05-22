using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RentApp.Server.Models.DTO.Product
{
    public class ProductCreateDTO
    {
        [Required]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Numele trebuie sa aiba intre 3 si 100 de caractere")]
        public string Name { get; set; } = string.Empty;

        public string Category { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "Descrierea nu poate avea mai mult de 500 caractere")]
        public string Description { get; set; } = string.Empty;

        [Range(0.01, 10000, ErrorMessage = "Pretul pe zi trebuie sa fie între 0.01 și 10.000 lei")]
        public decimal PricePerDay { get; set; }

        public bool Available { get; set; } = true;
        public string? ImagePath { get; set; }
        public string Location { get; set; }
    }
}
