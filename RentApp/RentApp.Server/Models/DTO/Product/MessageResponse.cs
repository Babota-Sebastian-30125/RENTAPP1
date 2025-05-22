namespace RentApp.Server.Models.DTO.Product
{
    public class MessageResponse<T>
    {
        public string Message { get; set; }
        public T Data { get; set; }
    }
}
