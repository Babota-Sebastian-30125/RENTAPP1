namespace RentApp.Server.Models.DTO.Rental
{
    public class RentalRequestDTO
    {
        public int ProductId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
