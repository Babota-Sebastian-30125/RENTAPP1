using System.ComponentModel.DataAnnotations;

namespace RentApp.Server.Models.DTO.User
{
    public class UpdateAccountDTO
    {
        public string? Name { get; set; }

        [EmailAddress(ErrorMessage = "Email invalid")]
        public string? Email { get; set; }

        public string? TelephoneNumber { get; set; }
    }
}
