
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RentApp.Server.Controllers;
using RentApp.Server.Models;
using RentApp.Server.Models.DTO.Favorite;
using RentApp.Server.Models.DTO.Product;
using RentApp.Server.Service;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
using IProductService = RentApp.Server.Models.DTO.Product.IProductService;

namespace RentApp.Tests.Controllers
{
    public class ProductsControllerTests
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
        public async Task GetProducts_ReturnsOkWithProducts()
        {
            // Arrange
            var expectedProducts = new List<ProductDTO>
            {
                new ProductDTO { Id = 1, Name = "Product 1", PricePerDay = 100 },
                new ProductDTO { Id = 2, Name = "Product 2", PricePerDay = 200 }
            };

            var productServiceMock = new Mock<IProductService>();
            productServiceMock.Setup(s => s.GetProductsAsync(
                It.IsAny<string>(), It.IsAny<ProductCategory?>(), It.IsAny<decimal?>(),
                It.IsAny<decimal?>(), It.IsAny<string>(), It.IsAny<Country?>(), It.IsAny<double?>()))
                .ReturnsAsync(expectedProducts);

            var controller = new ProductsController(productServiceMock.Object);
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Accept"] = "application/json";
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = await controller.GetProducts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<MessageResponse<IEnumerable<ProductDTO>>>(okResult.Value);
            Assert.Equal("Produse incarcate", response.Message);
            Assert.Equal(expectedProducts, response.Data);

        }

        [Fact]
        public async Task GetProducts_WithTextPlainAcceptHeader_ReturnsTextFormat()
        {
            // Arrange
            var products = new List<ProductDTO>
            {
                new ProductDTO { Id = 1, Name = "Product 1", PricePerDay = 100 },
                new ProductDTO { Id = 2, Name = "Product 2", PricePerDay = 200 }
            };

            var productServiceMock = new Mock<IProductService>();
            productServiceMock.Setup(s => s.GetProductsAsync(
                It.IsAny<string>(), It.IsAny<ProductCategory?>(), It.IsAny<decimal?>(),
                It.IsAny<decimal?>(), It.IsAny<string>(), It.IsAny<Country?>(), It.IsAny<double?>()))
                .ReturnsAsync(products);

            var controller = new ProductsController(productServiceMock.Object);
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Accept"] = "text/plain";
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = await controller.GetProducts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var textContent = Assert.IsType<string>(okResult.Value);
            Assert.Contains("Product 1 - 100", textContent);
            Assert.Contains("Product 2 - 200", textContent);
        }

        [Fact]
        public async Task GetProduct_ExistingId_ReturnsOkWithProduct()
        {
            // Arrange
            int productId = 1;
            var expectedProduct = new ProductDTO { Id = productId, Name = "Test Product", PricePerDay = 100 };

            var productServiceMock = new Mock<IProductService>();
            productServiceMock.Setup(s => s.GetProductByIdAsync(productId))
                .ReturnsAsync(expectedProduct);

            var controller = new ProductsController(productServiceMock.Object);

            // Act
            var result = await controller.GetProduct(productId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<MessageResponse<ProductDTO>>(okResult.Value);
            Assert.Equal("Produs gasit cu succes.", response.Message);
            Assert.Equal(expectedProduct, response.Data);

        }

        [Fact]
        public async Task GetProduct_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            int productId = 999;

            var productServiceMock = new Mock<IProductService>();
            productServiceMock.Setup(s => s.GetProductByIdAsync(productId))
                .ReturnsAsync((ProductDTO)null);

            var controller = new ProductsController(productServiceMock.Object);

            // Act
            var result = await controller.GetProduct(productId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var response = Assert.IsType<MessageResponse>(notFoundResult.Value);
            Assert.Equal($"Produsul cu ID-ul {productId} nu a fost gasit", response.Message);

        }

        [Fact]
        public async Task PostProduct_ValidData_ReturnsCreated()
        {
            // Arrange
            int userId = 1;
            var productDto = new ProductCreateWithFileDTO
            {
                Name = "New Product",
                Description = "Product Description",
                PricePerDay = 100,
                Category = ProductCategory.Electronice
            };

            var createdProduct = new ProductDTO { Id = 1, Name = productDto.Name, PricePerDay = productDto.PricePerDay };

            var productServiceMock = new Mock<IProductService>();
            productServiceMock.Setup(s => s.CreateProductAsync(productDto, userId))
                .ReturnsAsync(createdProduct);

            var controller = new ProductsController(productServiceMock.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = SetupUser(userId) }
            };

            // Act
            var result = await controller.PostProduct(productDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(ProductsController.GetProduct), createdAtActionResult.ActionName);
            Assert.Equal(createdProduct.Id, createdAtActionResult.RouteValues["id"]);

            var response = Assert.IsType<MessageResponse<int>>(createdAtActionResult.Value);
            Assert.Equal("Produs creat cu succes", response.Message);
            Assert.Equal(createdProduct.Id, response.Data);

        }

        [Fact]
        public async Task PostProduct_ServiceThrowsException_ReturnsBadRequest()
        {
            // Arrange
            int userId = 1;
            var productDto = new ProductCreateWithFileDTO
            {
                Name = "New Product",
                Description = "Product Description",
                PricePerDay = 100,
                Category = ProductCategory.Electronice
            };

            string errorMessage = "Eroare la crearea produsului";
            var productServiceMock = new Mock<IProductService>();
            productServiceMock.Setup(s => s.CreateProductAsync(productDto, userId))
                .ThrowsAsync(new Exception(errorMessage));

            var controller = new ProductsController(productServiceMock.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = SetupUser(userId) }
            };

            // Act
            var result = await controller.PostProduct(productDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<MessageResponse>(badRequestResult.Value);
            Assert.Contains(errorMessage, response.Message);
        }

            [Fact]
        public async Task UpdateProduct_ValidData_ReturnsOk()
        {
            // Arrange
            int userId = 1;
            int productId = 1;
            var productDto = new ProductCreateWithFileDTO
            {
                Name = "Updated Product",
                Description = "Updated Description",
                PricePerDay = 150,
                Category = ProductCategory.Electronice
            };

            var productServiceMock = new Mock<IProductService>();
            productServiceMock.Setup(s => s.UpdateProductAsync(productId, productDto, userId))
                .ReturnsAsync(true);

            var controller = new ProductsController(productServiceMock.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = SetupUser(userId) }
            };

            // Act
            var result = await controller.UpdateProduct(productId, productDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic response = okResult.Value;
            Assert.Equal("Produs actualizat cu succes", response.Message.ToString());
        }

        [Fact]
        public async Task UpdateProduct_UserNotOwner_ReturnsForbid()
        {
            // Arrange
            int userId = 1;
            int productId = 1;
            var productDto = new ProductCreateWithFileDTO
            {
                Name = "Updated Product",
                Description = "Updated Description",
                PricePerDay = 150,
                Category = ProductCategory.Electronice
            };

            var productServiceMock = new Mock<IProductService>();
            productServiceMock.Setup(s => s.UpdateProductAsync(productId, productDto, userId))
                .ReturnsAsync(false);

            var controller = new ProductsController(productServiceMock.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = SetupUser(userId) }
            };

            // Act
            var result = await controller.UpdateProduct(productId, productDto);

            // Assert
            Assert.IsType<ForbidResult>(result);


        }

        [Fact]
        public async Task DeleteProduct_UserIsOwner_ReturnsOk()
        {
            // Arrange
            int userId = 1;
            int productId = 1;

            var productServiceMock = new Mock<IProductService>();
            productServiceMock.Setup(s => s.DeleteProductAsync(productId, userId))
                .ReturnsAsync(true);

            var controller = new ProductsController(productServiceMock.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = SetupUser(userId) }
            };

            // Act
            var result = await controller.DeleteProduct(productId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<MessageResponse>(okResult.Value);
            Assert.Equal("Produs sters cu succes", response.Message);

        }

        [Fact]
        public async Task DeleteProduct_UserNotOwner_ReturnsForbid()
        {
            // Arrange
            int userId = 1;
            int productId = 1;

            var productServiceMock = new Mock<IProductService>();
            productServiceMock.Setup(s => s.DeleteProductAsync(productId, userId))
                .ReturnsAsync(false);

            var controller = new ProductsController(productServiceMock.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = SetupUser(userId) }
            };

            // Act
            var result = await controller.DeleteProduct(productId);

            // Assert
            Assert.IsType<ForbidResult>(result);

        }
    }
}