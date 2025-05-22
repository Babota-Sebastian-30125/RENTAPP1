
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RentApp.Server.Controllers;
using RentApp.Server.Models.DTO.Product;
using RentApp.Server.Models.DTO.Rental;
using RentApp.Server.Service;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace RentApp.Tests.Controllers
{
    public class RentalsControllerTests
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
        public async Task RentProduct_ValidRequest_ReturnsOkWithRentalId()
        {
            // Arrange
            int userId = 1;
            int expectedRentalId = 123;
            var rentalDto = new RentalRequestDTO
            {
                ProductId = 1,
                StartDate = DateTime.Now.AddDays(1),
                EndDate = DateTime.Now.AddDays(5)
            };

            var rentalServiceMock = new Mock<IRentalService>();
            rentalServiceMock.Setup(s => s.RentProductAsync(rentalDto, userId))
                .ReturnsAsync(expectedRentalId);

            var controller = new RentalsController(rentalServiceMock.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = SetupUser(userId) }
            };

            // Act
            var result = await controller.RentProduct(rentalDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<MessageResponse>(okResult.Value);
            Assert.Equal("Inchiriere realizata cu succes", response.Message);
            Assert.Equal(expectedRentalId, response.RentalId);
        }

        [Fact]
        public async Task RentProduct_ServiceThrowsException_ReturnsBadRequest()
        {
            // Arrange
            int userId = 1;
            string errorMessage = "Produsul este deja inchiriat pentru perioada selectată";
            var rentalDto = new RentalRequestDTO
            {
                ProductId = 1,
                StartDate = DateTime.Now.AddDays(1),
                EndDate = DateTime.Now.AddDays(5)
            };

            var rentalServiceMock = new Mock<IRentalService>();
            rentalServiceMock.Setup(s => s.RentProductAsync(rentalDto, userId))
                .ThrowsAsync(new Exception(errorMessage));

            var controller = new RentalsController(rentalServiceMock.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = SetupUser(userId) }
            };

            // Act
            var result = await controller.RentProduct(rentalDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<MessageResponse>(badRequestResult.Value);
            Assert.Equal(errorMessage, response.Message.ToString());
        }

        [Fact]
        public async Task GetMyRentals_ReturnsUserRentals()
        {
            // Arrange
            int userId = 1;
            var expectedRentals = new List<RentalListItemDTO>
            {
                new RentalListItemDTO {RentalId = 1,
                ProductName = "Produs 1",
                StartDate = DateTime.Now.AddDays(1),
                EndDate = DateTime.Now.AddDays(5),
                TotalPrice = 500,
                Status = "Active"  },

                new RentalListItemDTO {
                RentalId = 2,
                ProductName = "Produs 2",
                StartDate = DateTime.Now.AddDays(10),
                EndDate = DateTime.Now.AddDays(15),
                TotalPrice = 800,
                Status = "Completed"  }
            };

            var rentalServiceMock = new Mock<IRentalService>();
            rentalServiceMock.Setup(s => s.GetMyRentalsAsync(userId))
                .ReturnsAsync(expectedRentals);

            var controller = new RentalsController(rentalServiceMock.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = SetupUser(userId) }
            };

            // Act
            var result = await controller.GetMyRentals();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var rentals = Assert.IsAssignableFrom<IEnumerable<RentalListItemDTO>>(okResult.Value);
            Assert.Equal(expectedRentals.Count, rentals.Count());
            Assert.Equal(expectedRentals.First().RentalId, rentals.First().RentalId);
        }

        [Fact]
        public async Task CancelRental_ExistingRental_ReturnsOkAndCancelsRental()
        {
            // Arrange
            int userId = 1;
            int rentalId = 1;

            var rentalServiceMock = new Mock<IRentalService>();
            rentalServiceMock.Setup(s => s.CancelRentalAsync(rentalId, userId))
                .ReturnsAsync(true);

            var controller = new RentalsController(rentalServiceMock.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = SetupUser(userId) }
            };

            // Act
            var result = await controller.CancelRental(rentalId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<MessageResponse>(okResult.Value);
            Assert.Equal("Inchirierea a fost anulata", response.Message.ToString());
        }

        [Fact]
        public async Task CancelRental_NonExistingRental_ReturnsNotFound()
        {
            // Arrange
            int userId = 1;
            int rentalId = 999;

            var rentalServiceMock = new Mock<IRentalService>();
            rentalServiceMock.Setup(s => s.CancelRentalAsync(rentalId, userId))
                .ReturnsAsync(false);

            var controller = new RentalsController(rentalServiceMock.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = SetupUser(userId) }
            };

            // Act
            var result = await controller.CancelRental(rentalId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task CancelRental_ServiceThrowsException_ReturnsBadRequest()
        {
            // Arrange
            int userId = 1;
            int rentalId = 1;
            string errorMessage = "Nu poți anula o închiriere care a început deja";

            var rentalServiceMock = new Mock<IRentalService>();
            rentalServiceMock.Setup(s => s.CancelRentalAsync(rentalId, userId))
                .ThrowsAsync(new Exception(errorMessage));

            var controller = new RentalsController(rentalServiceMock.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = SetupUser(userId) }
            };

            // Act
            var result = await controller.CancelRental(rentalId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<MessageResponse>(badRequestResult.Value);
            Assert.Equal(errorMessage, response.Message.ToString());
        }


        [Fact]
        public async Task GetProductDetails_NonExistingProduct_ReturnsNotFound()
        {
            // Arrange
            int productId = 999;

            var rentalServiceMock = new Mock<IRentalService>();
            rentalServiceMock
                .Setup(s => s.GetProductDetailsAsync(productId))
                .ReturnsAsync((ProductRentalDetailsDTO?)null);

            var controller = new RentalsController(rentalServiceMock.Object);

            // Act
            var result = await controller.GetProductDetails(productId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var response = Assert.IsType<MessageResponse>(notFoundResult.Value);
            Assert.Equal("Produsul nu a fost gasit", response.Message.ToString());
        }

    }
}