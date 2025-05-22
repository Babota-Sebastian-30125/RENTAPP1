using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentApp.Data;
using RentApp.Models;
using RentApp.Server.Models.DTO.Auth;
using RentApp.Server.Service;


namespace RentApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly RentDbContext _context;
        private readonly IAuthService _authService;

        public AuthController(RentDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _context.Users.AnyAsync(u => u.email == dto.Email))
                return BadRequest(new ErrorResponse{ error = "Emailul este deja folosit", code = "EMAIL_EXISTS" });

            var user = new User
            {
                Name = dto.Name,
                email = dto.Email,
                telephoneNumber = dto.TelephoneNumber,
                password = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Created("", new { message = "Cont creat cu succes!" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.email == dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.password))
                return Unauthorized(new ErrorResponse{ error = "Email sau parola incorecte", code = "INVALID_CREDENTIALS" });

            var token = _authService.GenerateJwtToken(user);

            return Ok(new LoginResponseDTO
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.email,
                TelephoneNumber = user.telephoneNumber,
                Token = token
            });
        }

      

      
    }
}