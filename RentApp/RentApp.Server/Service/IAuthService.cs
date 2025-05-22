using RentApp.Models;

namespace RentApp.Server.Service
{
    public interface IAuthService
    {
        string GenerateJwtToken(User user);
    }
}
