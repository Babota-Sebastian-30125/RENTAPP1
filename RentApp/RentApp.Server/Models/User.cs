using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace RentApp.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string Name { get; set; }
        public string telephoneNumber { get; set; }
        public string email { get; set; }
        public string password { get; set; }

        // Navigation property pentru relatie 1:N
        public ICollection<Product> Products { get; set; }

        // Navigation property pentru relatie 1:N
        public ICollection<Rental> Rentals { get; set; }
    }
}