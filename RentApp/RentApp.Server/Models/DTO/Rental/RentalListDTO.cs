

namespace RentApp.Server.Models.DTO.Rental
{
    public class RentalListItemDTO
    {
        public int RentalId { get; set; }
        public string ProductName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } 
    }
}
