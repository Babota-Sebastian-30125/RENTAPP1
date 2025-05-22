using System.ComponentModel.DataAnnotations;

namespace RentApp.Server.Models.DTO.Auth
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "Email-ul este obligatoriu")]
        [EmailAddress(ErrorMessage = "Email invalid")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Parola este obligatorie")]
        public string Password { get; set; }
    }
}
