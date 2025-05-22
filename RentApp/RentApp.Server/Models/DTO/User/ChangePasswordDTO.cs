using System.ComponentModel.DataAnnotations;

namespace RentApp.Server.Models.DTO.User
{
    public class ChangePasswordDTO
    {
        [Required(ErrorMessage = "Parola veche este obligatorie")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Parola noua este obligatorie")]
        [MinLength(8, ErrorMessage = "Parola noua trebuie sa aiba minim 8 caractere")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*]).+$",
            ErrorMessage = "Parola noua trebuie sa contina cel putin o litera mare, o cifra si un caracter special")]
        public string NewPassword { get; set; }
    }
}
