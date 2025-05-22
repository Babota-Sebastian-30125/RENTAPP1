using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using RentApp.Controllers;
using RentApp.Data;
using RentApp.Models;
using RentApp.Server.Models.DTO.Auth;
using RentApp.Server.Service;
using System.Threading.Tasks;
using Xunit;

namespace RentApp.Tests.Controllers
{
    public class AuthControllerTests
    {
        // Test pentru înregistrarea unui utilizator nou cu succes
        [Fact]
        public async Task Register_ValidData_ReturnsCreated()
        {
            var options = new DbContextOptionsBuilder<RentDbContext>()
                .UseInMemoryDatabase("Register_ValidData_ReturnsCreated")
                .Options;

            using var context = new RentDbContext(options);

            var authServiceMock = new Mock<IAuthService>();
            var controller = new AuthController(context, authServiceMock.Object);

            var registerDto = new RegisterDTO
            {
                Name = "Test User",
                Email = "test@example.com",
                TelephoneNumber = "0712345678",
                Password = "Password123!"
            };

            var result = await controller.Register(registerDto);

            var createdResult = Assert.IsType<CreatedResult>(result);
            Assert.Equal("", createdResult.Location);

            var user = await context.Users.FirstOrDefaultAsync(u => u.email == registerDto.Email);
            Assert.NotNull(user);
            Assert.Equal(registerDto.Name, user.Name);
            Assert.Equal(registerDto.Email, user.email);
            Assert.Equal(registerDto.TelephoneNumber, user.telephoneNumber);
            Assert.True(BCrypt.Net.BCrypt.Verify(registerDto.Password, user.password));
        }

        // Test pentru înregistrare cu email deja existent
        [Fact]
        public async Task Register_EmailExists_ReturnsBadRequest()
        {
            var options = new DbContextOptionsBuilder<RentDbContext>()
                .UseInMemoryDatabase("Register_EmailExists_ReturnsBadRequest")
                .Options;

            using var context = new RentDbContext(options);

            context.Users.Add(new User
            {
                Name = "Existing User",
                email = "existing@example.com",
                telephoneNumber = "0712345678",
                password = "hashedpassword"
            });
            await context.SaveChangesAsync();

            var authServiceMock = new Mock<IAuthService>();
            var controller = new AuthController(context, authServiceMock.Object);

            var registerDto = new RegisterDTO
            {
                Name = "Test User",
                Email = "existing@example.com",
                TelephoneNumber = "0723456789",
                Password = "Password123!"
            };

            var result = await controller.Register(registerDto);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorResponse = Assert.IsType<ErrorResponse>(badRequestResult.Value);

            Assert.Equal("EMAIL_EXISTS", errorResponse.code);
        }

        // Test pentru login cu credențiale valide
        [Fact]
        public async Task Login_ValidCredentials_ReturnsOkWithToken()
        {
            var options = new DbContextOptionsBuilder<RentDbContext>()
                .UseInMemoryDatabase("Login_ValidCredentials_ReturnsOkWithToken")
                .Options;

            string testPassword = "Password123!";
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(testPassword);
            string testToken = "test.jwt.token";

            using var context = new RentDbContext(options);

            var user = new User
            {
                UserId = 1,
                Name = "Test User",
                email = "test@example.com",
                telephoneNumber = "0712345678",
                password = hashedPassword
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var authServiceMock = new Mock<IAuthService>();
            authServiceMock.Setup(s => s.GenerateJwtToken(It.IsAny<User>())).Returns(testToken);

            var controller = new AuthController(context, authServiceMock.Object);

            var loginDto = new LoginDTO
            {
                Email = "test@example.com",
                Password = testPassword
            };

            var result = await controller.Login(loginDto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var loginResponse = Assert.IsType<LoginResponseDTO>(okResult.Value);

            Assert.Equal(user.UserId, loginResponse.UserId);
            Assert.Equal(user.Name, loginResponse.Name);
            Assert.Equal(user.email, loginResponse.Email);
            Assert.Equal(user.telephoneNumber, loginResponse.TelephoneNumber);
            Assert.Equal(testToken, loginResponse.Token);
        }

        // Test pentru login cu email inexistent
        [Fact]
        public async Task Login_EmailNotFound_ReturnsUnauthorized()
        {
            var options = new DbContextOptionsBuilder<RentDbContext>()
                .UseInMemoryDatabase("Login_EmailNotFound_ReturnsUnauthorized")
                .Options;

            using var context = new RentDbContext(options);

            var authServiceMock = new Mock<IAuthService>();
            var controller = new AuthController(context, authServiceMock.Object);

            var loginDto = new LoginDTO
            {
                Email = "nonexistent@example.com",
                Password = "Password123!"
            };

            var result = await controller.Login(loginDto);

            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var errorResponse = Assert.IsType<ErrorResponse>(unauthorizedResult.Value);

            Assert.Equal("INVALID_CREDENTIALS", errorResponse.code);
        }

        // Test pentru login cu parolă incorectă
        [Fact]
        public async Task Login_IncorrectPassword_ReturnsUnauthorized()
        {
            var options = new DbContextOptionsBuilder<RentDbContext>()
                .UseInMemoryDatabase("Login_IncorrectPassword_ReturnsUnauthorized")
                .Options;

            string correctPassword = "Password123!";
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(correctPassword);

            using var context = new RentDbContext(options);

            var user = new User
            {
                UserId = 1,
                Name = "Test User",
                email = "test@example.com",
                telephoneNumber = "0712345678",
                password = hashedPassword
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var authServiceMock = new Mock<IAuthService>();
            var controller = new AuthController(context, authServiceMock.Object);

            var loginDto = new LoginDTO
            {
                Email = "test@example.com",
                Password = "WrongPassword123!"
            };

            var result = await controller.Login(loginDto);

            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var errorResponse = Assert.IsType<ErrorResponse>(unauthorizedResult.Value);

            Assert.Equal("INVALID_CREDENTIALS", errorResponse.code);
        }
    }
}
