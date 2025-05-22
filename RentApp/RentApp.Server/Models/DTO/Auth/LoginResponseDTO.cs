namespace RentApp.Server.Models.DTO.Auth
{

    public class LoginResponseDTO
    {

        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string TelephoneNumber { get; set; }
        public string Token { get; set; } // pt jwt
    }
}

    
