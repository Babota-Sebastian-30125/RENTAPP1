
using RentApp.Server.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentApp.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Category { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime AddedAt { get; set; }
        public bool Available { get; set; }
        public string ImagePath { get; set; }
        public string Location { get; set; }



        [Column(TypeName = "Money")]
        public decimal PricePerDay { get; set; }

        // Foreign Key
        public int UserId { get; set; }

        // Navigation Property 
        [ForeignKey("UserId")]
        public User? user { get; set; }

        // Navigation property pentru relatie 1:N
        public ICollection<Rental>? Rentals { get; set; }

        public ICollection<Review>? Reviews { get; set; }
    }
}