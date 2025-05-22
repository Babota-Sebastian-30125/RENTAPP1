
using RentApp.Server.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace RentApp.Models
{

    public class Rental
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? RentalId { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        
        [Column(TypeName = "Money")]
        public decimal TotalPrice { get; set; }
        public DateTime RegisterDate { get; set; }
        public States Status { get; set; }

        // Foreign Key
        public int ProductId { get; set; }

        // Navigation Property 
        // [ForeignKey("Id")]
        public Product product { get; set; }

        // Foreign Key
        public int UserId { get; set; }

        // Navigation Property
        [ForeignKey("UserId")]
        public User user { get; set; }
    }
}