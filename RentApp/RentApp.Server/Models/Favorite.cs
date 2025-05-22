using RentApp.Models;

namespace RentApp.Server.Models
{
    public class Favorite
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}
