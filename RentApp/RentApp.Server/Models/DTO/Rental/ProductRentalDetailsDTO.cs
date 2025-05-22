namespace RentApp.Server.Models.DTO.Rental
{
    public class ProductRentalDetailsDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal PricePerDay { get; set; }
        public string Description { get; set; }
        public bool Available { get; set; }
    }
}
