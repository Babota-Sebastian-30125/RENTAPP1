using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentApp.Controllers;
using RentApp.Data;
using RentApp.Models;
using RentApp.Server.Models.DTO.User;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Xunit;

namespace RentApp.Tests.Controllers
{
    public class UserControllerTests
    {
        private ClaimsPrincipal SetupUser(int userId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            return new ClaimsPrincipal(identity);
        }

        [Fact]
        public async Task GetProfile_UserExists_ReturnsOkWithUserData()
        {
            int userId = 1;
            var testUser = new User
            {
                UserId = userId,
                Name = "Test User",
                email = "test@example.com",
                telephoneNumber = "0712345678",
                password = "hashedpassword"
            };

            var options = new DbContextOptionsBuilder<RentDbContext>()
                .UseInMemoryDatabase(databaseName: "GetProfile_UserExists")
                .Options;

            using var context = new RentDbContext(options);
            context.Users.Add(testUser);
            await context.SaveChangesAsync();

            var controller = new UserController(context);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = SetupUser(userId) }
            };

            var result = await controller.GetProfile();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var userProfile = Assert.IsType<User>(okResult.Value);
            Assert.Equal(testUser.UserId, (int)userProfile.UserId);
            Assert.Equal(testUser.Name, (string)userProfile.Name);
            Assert.Equal(testUser.email, (string)userProfile.email);
            Assert.Equal(testUser.telephoneNumber, (string)userProfile.telephoneNumber);
        }

        [Fact]
        public async Task GetProfile_UserNotFound_ReturnsNotFound()
        {
            int userId = 999;

            var options = new DbContextOptionsBuilder<RentDbContext>()
                .UseInMemoryDatabase("GetProfile_UserNotFound")
                .Options;

            using var context = new RentDbContext(options);

            var controller = new UserController(context);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = SetupUser(userId) }
            };

            var result = await controller.GetProfile();

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var response = Assert.IsType<MessageResponse>(notFoundResult.Value);
            Assert.Equal("Utilizatorul nu a fost gasit", response.Error);
        }

        [Fact]
        public async Task UpdateProfile_ValidData_ReturnsOkAndUpdatesUser()
        {
            int userId = 1;
            var testUser = new User
            {
                UserId = userId,
                Name = "Original Name",
                email = "original@example.com",
                telephoneNumber = "0712345678",
                password = "hashedpassword"
            };

            var updateDto = new UpdateAccountDTO
            {
                Name = "Updated Name",
                Email = "updated@example.com",
                TelephoneNumber = "0723456789"
            };

            var options = new DbContextOptionsBuilder<RentDbContext>()
                .UseInMemoryDatabase("UpdateProfile_ValidData")
                .Options;

            using var context = new RentDbContext(options);
            context.Users.Add(testUser);
            await context.SaveChangesAsync();

            var controller = new UserController(context);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = SetupUser(userId) }
            };

            var result = await controller.UpdateProfile(updateDto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<MessageResponse>(okResult.Value);
            Assert.Equal("Contul a fost actualizat cu succes", response.Message);

            var updatedUser = await context.Users.FindAsync(userId);
            Assert.Equal(updateDto.Name, updatedUser.Name);
            Assert.Equal(updateDto.Email, updatedUser.email);
            Assert.Equal(updateDto.TelephoneNumber, updatedUser.telephoneNumber);
        }

        [Fact]
        public async Task ChangePassword_ValidCredentials_ReturnsOkAndUpdatesPassword()
        {
            int userId = 1;
            string oldPassword = "OldPassword123";
            string newPassword = "NewPassword123";
            string hashedOldPassword = BCrypt.Net.BCrypt.HashPassword(oldPassword);

            var changePasswordDto = new ChangePasswordDTO
            {
                OldPassword = oldPassword,
                NewPassword = newPassword
            };

            var testUser = new User
            {
                UserId = userId,
                Name = "Test User",
                email = "test@example.com",
                telephoneNumber = "0712345678",
                password = hashedOldPassword
            };

            var options = new DbContextOptionsBuilder<RentDbContext>()
                .UseInMemoryDatabase("ChangePassword_Valid")
                .Options;

            using var context = new RentDbContext(options);
            context.Users.Add(testUser);
            await context.SaveChangesAsync();

            var controller = new UserController(context);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = SetupUser(userId) }
            };

            var result = await controller.ChangePassword(changePasswordDto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<MessageResponse>(okResult.Value);
            Assert.Equal("Parola a fost schimbata cu succes", response.Message);

            var updatedUser = await context.Users.FindAsync(userId);
            Assert.True(BCrypt.Net.BCrypt.Verify(newPassword, updatedUser.password));
        }

        [Fact]
        public async Task ChangePassword_IncorrectOldPassword_ReturnsBadRequest()
        {
            int userId = 1;
            string actualPassword = "ActualPassword123";
            string incorrectOldPassword = "WrongPassword123";
            string newPassword = "NewPassword123";
            string hashedActualPassword = BCrypt.Net.BCrypt.HashPassword(actualPassword);

            var changePasswordDto = new ChangePasswordDTO
            {
                OldPassword = incorrectOldPassword,
                NewPassword = newPassword
            };

            var testUser = new User
            {
                UserId = userId,
                Name = "Test User",
                email = "test@example.com",
                telephoneNumber = "0712345678",
                password = hashedActualPassword
            };

            var options = new DbContextOptionsBuilder<RentDbContext>()
                .UseInMemoryDatabase("ChangePassword_Incorrect")
                .Options;

            using var context = new RentDbContext(options);
            context.Users.Add(testUser);
            await context.SaveChangesAsync();

            var controller = new UserController(context);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = SetupUser(userId) }
            };

            var result = await controller.ChangePassword(changePasswordDto);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<MessageResponse>(badRequestResult.Value);
            Assert.Equal("Parola veche este incorecta", response.Error);

            var user = await context.Users.FindAsync(userId);
            Assert.Equal(hashedActualPassword, user.password);
        }

        [Fact]
        public async Task DeleteAccount_UserExists_ReturnsOkAndDeletesAccount()
        {
            int userId = 1;
            var testUser = new User
            {
                UserId = userId,
                Name = "Test User",
                email = "test@example.com",
                telephoneNumber = "0712345678",
                password = "hashedpassword",
                Products = new List<Product>(),
                Rentals = new List<Rental>()
            };

            var options = new DbContextOptionsBuilder<RentDbContext>()
    .UseInMemoryDatabase(databaseName: "DeleteAccount_UserExists_ReturnsOkAndDeletesAccount")
    .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning))
    .Options;


            using var context = new RentDbContext(options);
            context.Users.Add(testUser);
            await context.SaveChangesAsync();

            var controller = new UserController(context);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = SetupUser(userId) }
            };

            var result = await controller.DeleteAccount();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<MessageResponse>(okResult.Value);
            Assert.Equal("Contul a fost sters cu succes", response.Message);

            var deletedUser = await context.Users.FindAsync(userId);
            Assert.Null(deletedUser);
        }
        [Fact]
        public async Task ChangePassword_NewPasswordSameAsOld_ReturnsBadRequest()
        {
            int userId = 1;
            string password = "SamePassword123";
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var changePasswordDto = new ChangePasswordDTO
            {
                OldPassword = password,
                NewPassword = password // aceeași parolă
            };

            var testUser = new User
            {
                UserId = userId,
                Name = "Test User",                // completat
                email = "test@example.com",       // completat
                telephoneNumber = "0712345678",   // completat
                password = hashedPassword
            };

            var options = new DbContextOptionsBuilder<RentDbContext>()
                .UseInMemoryDatabase(databaseName: "ChangePassword_NewPasswordSameAsOld_ReturnsBadRequest")
                .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            using var context = new RentDbContext(options);
            context.Users.Add(testUser);
            await context.SaveChangesAsync();

            var controller = new UserController(context);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = SetupUser(userId) }
            };

            var result = await controller.ChangePassword(changePasswordDto);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<MessageResponse>(badRequestResult.Value);
            Assert.Equal("Parola noua trebuie sa fie diferita de cea veche", response.Error.ToString());
        }

    }
}
