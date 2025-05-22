using System.ComponentModel.DataAnnotations;


namespace RentApp.Server.Models.DTO.Auth
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "Numele este obligatoriu")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email-ul este obligatoriu")]
        [EmailAddress(ErrorMessage = "Email invalid")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Numarul de telefon este obligatoriu")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Numarul de telefon trebuie sa contina exact 10 cifre")]
        public string TelephoneNumber { get; set; }

        [Required(ErrorMessage = "Parola este obligatorie")]
        [MinLength(8, ErrorMessage = "Parola trebuie să aiba minim 8 caractere")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*]).+$",
            ErrorMessage = "Parola trebuie sa contina cel putin o litera mare, o cifra si un caracter special")]
        public string Password { get; set; }
    }
}