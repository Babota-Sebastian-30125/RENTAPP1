/*
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RentApp.Server.Controllers;
using RentApp.Server.Models.DTO.Review;
using RentApp.Server.Service;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace RentApp.Tests.Controllers
{
    public class ReviewsControllerTests
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
        public async Task AddReview_ValidRequest_ReturnsOk()
        {
            // Arrange
            int userId = 1;
            int productId = 2;
            var reviewDto = new ReviewDTO
            {
                Rating = 5,
                Comment = "Excelent produs!"
            };

            var reviewServiceMock = new Mock<IReviewService>();
            reviewServiceMock.Setup(s => s.AddReviewAsync(productId, userId, reviewDto))
                .ReturnsAsync(true);

            var controller = new ReviewsController(reviewServiceMock.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = SetupUser(userId) }
            };

            // Act
            var result = await controller.AddReview(productId, reviewDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic response = okResult.Value;
            Assert.Equal("Review adaugat cu succes!", response.message.ToString());
        }

        [Fact]
        public async Task AddReview_UserNotRented_ReturnsBadRequest()
        {
            // Arrange
            int userId = 1;
            int productId = 2;
            var reviewDto = new ReviewDTO
            {
                Rating = 4,
                Comment = "Bun produs!"
            };

            var reviewServiceMock = new Mock<IReviewService>();
            reviewServiceMock.Setup(s => s.AddReviewAsync(productId, userId, reviewDto))
                .ReturnsAsync(false);

            var controller = new ReviewsController(reviewServiceMock.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = SetupUser(userId) }
            };

            // Act
            var result = await controller.AddReview(productId, reviewDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            dynamic response = badRequestResult.Value;
            Assert.Equal("Nu poti lasa review daca nu ai inchiriat produsul", response.message.ToString());
        }

        [Fact]
        public async Task AddReview_ServiceThrowsException_ReturnsBadRequest()
        {
            // Arrange
            int userId = 1;
            int productId = 2;
            string errorMessage = "Ai lăsat deja un review pentru acest produs";
            var reviewDto = new ReviewDTO
            {
                Rating = 3,
                Comment = "Produs decent."
            };

            var reviewServiceMock = new Mock<IReviewService>();
            reviewServiceMock.Setup(s => s.AddReviewAsync(productId, userId, reviewDto))
                .ThrowsAsync(new Exception(errorMessage));

            var controller = new ReviewsController(reviewServiceMock.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = SetupUser(userId) }
            };

            // Act
            var result = await controller.AddReview(productId, reviewDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            dynamic response = badRequestResult.Value;
            Assert.Equal(errorMessage, response.message.ToString());
        }

        [Fact]
        public async Task GetReviewsForProduct_ReturnsReviews()
        {
            // Arrange
            int productId = 1;
            var expectedReviews = new List<ReviewResponseDTO>
            {
                new ReviewResponseDTO { Id = 1, Rating = 5, Comment = "Excelent!", UserName = "User1" },
                new ReviewResponseDTO { Id = 2, Rating = 4, Comment = "Foarte bun!", UserName = "User2" }
            };

            var reviewServiceMock = new Mock<IReviewService>();
            reviewServiceMock.Setup(s => s.GetReviewsForProductAsync(productId))
                .ReturnsAsync(expectedReviews);

            var controller = new ReviewsController(reviewServiceMock.Object);

            // Act
            var result = await controller.GetReviewsForProduct(productId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var reviews = Assert.IsAssignableFrom<IEnumerable<ReviewDTO>>(okResult.Value);
            Assert.Equal(expectedReviews, reviews);
        }

        [Fact]
        public async Task GetAverageStars_ReturnsCorrectAverage()
        {
            // Arrange
            int productId = 1;
            double expectedAverage = 4.5;

            var reviewServiceMock = new Mock<IReviewService>();
            reviewServiceMock.Setup(s => s.GetAverageStarsAsync(productId))
                .ReturnsAsync(expectedAverage);

            var controller = new ReviewsController(reviewServiceMock.Object);

            // Act
            var result = await controller.GetAverageStars(productId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic response = okResult.Value;
            Assert.Equal(expectedAverage, response.average);
        }
    }
}
*/