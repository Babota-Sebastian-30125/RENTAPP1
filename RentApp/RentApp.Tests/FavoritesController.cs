using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RentApp.Models;
using RentApp.Server.Controllers;
using RentApp.Server.Models.DTO.Favorite;
using RentApp.Server.Service;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace RentApp.Tests.Controllers
{
    public class FavoritesControllerTests
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
        public async Task ToggleFavorite_AddsToFavorites_ReturnsOk()
        {
            int userId = 1;
            int productId = 2;
            bool wasAdded = true;

            var favoriteServiceMock = new Mock<IFavoriteService>();
            favoriteServiceMock.Setup(s => s.ToggleFavoriteAsync(userId, productId))
                .ReturnsAsync(wasAdded);

            var controller = new FavoritesController(favoriteServiceMock.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = SetupUser(userId) }
            };

            var result = await controller.ToggleFavorite(productId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<MessageResponse>(okResult.Value);
            Assert.Equal("Adaugat la favorite", response.Message);
        }

        [Fact]
        public async Task ToggleFavorite_RemovesFromFavorites_ReturnsOk()
        {
            int userId = 1;
            int productId = 2;
            bool wasAdded = false;

            var favoriteServiceMock = new Mock<IFavoriteService>();
            favoriteServiceMock.Setup(s => s.ToggleFavoriteAsync(userId, productId))
                .ReturnsAsync(wasAdded);

            var controller = new FavoritesController(favoriteServiceMock.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = SetupUser(userId) }
            };

            var result = await controller.ToggleFavorite(productId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<MessageResponse>(okResult.Value);
            Assert.Equal("Sters din favorite", response.Message);
        }

        [Fact]
        public async Task GetFavorites_ReturnsUserFavorites()
        {
            int userId = 1;
            var expectedFavorites = new List<Product>
            {
                new Product { Id = 1, Name = "Produs 1", PricePerDay = 100 },
                new Product { Id = 2, Name = "Produs 2", PricePerDay = 200 }
            };

            var favoriteServiceMock = new Mock<IFavoriteService>();
            favoriteServiceMock.Setup(s => s.GetUserFavoritesAsync(userId))
                .ReturnsAsync(expectedFavorites);

            var controller = new FavoritesController(favoriteServiceMock.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = SetupUser(userId) }
            };

            var result = await controller.GetFavorites();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var favorites = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);
            Assert.Equal(expectedFavorites, favorites);
        }

        [Fact]
        public async Task RemoveFavorite_ProductFound_ReturnsOk()
        {
            int userId = 1;
            int productId = 2;
            bool wasRemoved = true;

            var favoriteServiceMock = new Mock<IFavoriteService>();
            favoriteServiceMock.Setup(s => s.RemoveFavoriteAsync(userId, productId))
                .ReturnsAsync(wasRemoved);

            var controller = new FavoritesController(favoriteServiceMock.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = SetupUser(userId) }
            };

            var result = await controller.RemoveFavorite(productId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<MessageResponse>(okResult.Value);
            Assert.Equal("Produsul a fost scos din favorite", response.Message);
        }

        [Fact]
        public async Task RemoveFavorite_ProductNotFound_ReturnsNotFound()
        {
            int userId = 1;
            int productId = 2;
            bool wasRemoved = false;

            var favoriteServiceMock = new Mock<IFavoriteService>();
            favoriteServiceMock.Setup(s => s.RemoveFavoriteAsync(userId, productId))
                .ReturnsAsync(wasRemoved);

            var controller = new FavoritesController(favoriteServiceMock.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = SetupUser(userId) }
            };

            var result = await controller.RemoveFavorite(productId);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var response = Assert.IsType<MessageResponse>(notFoundResult.Value);
            Assert.Equal("Produsul nu a fost gasit in favorite", response.Message);
        }
    }
}
