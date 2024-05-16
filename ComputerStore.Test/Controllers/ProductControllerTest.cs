using AutoMapper;
using ComputerStore.Controllers;
using ComputerStore.DTO;
using ComputerStore.Interfaces;
using ComputerStore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ComputerStore.Tests.Controllers
{
    public class ProductControllerTests
    {
        private readonly ProductController _productController;
        private readonly Mock<IProductInterface> _productInterfaceMock;
        private readonly Mock<IMapper> _mapperMock;

        public ProductControllerTests()
        {
            _productInterfaceMock = new Mock<IProductInterface>();
            _mapperMock = new Mock<IMapper>();
            _productController = new ProductController(_productInterfaceMock.Object, _mapperMock.Object);
        }

        [Fact]
        public void GetProducts_ReturnsOkResult()
        {
            // Arrange
            var gpuCategory = new Category
            {
                CategoryId = 1,
                Name = "GPU",
                Description = "Graphics Processing Unit"
            };

            var products = new List<Product>
            {
            new Product
            {
                ProductId = 1,
                Name = "Product 1",
                Discription = "Description of Product 1",
                Price = 99.99m,
                Quantity = 10,
                ProductCategories = new List<Category>()
            }
         };
            _productInterfaceMock.Setup(repo => repo.GetProducts()).Returns(products);

            // Act
            var result = _productController.GetProducts() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.Equal(products, result.Value);
        }

        [Fact]
        public void GetProduct_WithValidId_ReturnsOkResult()
        {
            // Arrange

            var productDTO = new ProductDTO
            {
                Name = "Sample Product",
                Discription = "This is a sample product DTO",
                Price = 100,
                Quantity = 10,
                ProductCategories = new List<string> { "Category1", "Category2" }
            };
            var gpuCategory = new Category
            {
                CategoryId = 1,
                Name = "GPU",
                Description = "Graphics Processing Unit"
            };

            var product = new Product
            {
                ProductId = 1,
                Name = "Sample Product",
                Discription = "Sample Description",
                Price = 100,
                Quantity = 10,
                ProductCategories = new List<Category>()
            };
            product.ProductCategories.Add(gpuCategory);
            _productInterfaceMock.Setup(repo => repo.ProductExists(product.ProductId)).Returns(true);
            _productInterfaceMock.Setup(repo => repo.GetProduct(product.ProductId)).Returns(product);
            _mapperMock.Setup(mapper => mapper.Map<ProductDTO>(product)).Returns(productDTO);

            // Act
            var result = _productController.GetProduct(product.ProductId) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.Equal(productDTO, result.Value);
        }

        [Fact]
        public void GetProduct_WithInvalidId_ReturnsNotFoundResult()
        {
            // Arrange
            var productId = 999;
            _productInterfaceMock.Setup(repo => repo.ProductExists(productId)).Returns(false);

            // Act
            var result = _productController.GetProduct(productId) as NotFoundObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
            Assert.Equal("Id not found", result.Value);
        }

        [Fact]
        public void CreateProduct_WithValidInput_ReturnsCreatedResult()
        {
            // Arrange
            var productDTO =new ProductDTO
            {
                Name = "Sample Product",
                Discription = "This is a sample product DTO",
                Price = 100,
                Quantity = 10,
                ProductCategories = new List<string> { "Category1", "Category2" }
            };
            var gpuCategory = new Category
            {
                CategoryId = 1,
                Name = "GPU",
                Description = "Graphics Processing Unit"
            };

            var product = new Product
            {
                ProductId = 1,
                Name = "Sample Product",
                Discription = "Sample Description",
                Price = 100,
                Quantity = 10,
                ProductCategories = new List<Category>()
            };
            product.ProductCategories.Add(gpuCategory);
            _mapperMock.Setup(mapper => mapper.Map<Product>(productDTO)).Returns(product);
            _productInterfaceMock.Setup(repo => repo.CreateProduct(It.IsAny<ProductDTO>())).Returns(true);

            // Act
            var result = _productController.CreateProduct(productDTO) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.Equal("Product Created Successfully", result.Value);
        }


        [Fact]
        public void CreateProduct_WithInvalidInput_ReturnsBadRequestResult()
        {
            // Arrange

            // Act
            var result = _productController.CreateProduct(null) as BadRequestResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        }

        [Fact]
        public void UpdateProduct_WithValidInput_ReturnsOkResult()
        {
            // Arrange
            var updatedProductDTO = new UpdateProductDTO() { /* create sample updated product DTO */ };
            var updatedProduct = new Product() { /* create sample updated product */ };
            _productInterfaceMock.Setup(repo => repo.ProductExists(updatedProductDTO.Id)).Returns(true);
            _mapperMock.Setup(mapper => mapper.Map<Product>(updatedProductDTO)).Returns(updatedProduct);
            _productInterfaceMock.Setup(repo => repo.UpdateProduct(updatedProduct)).Returns(true);

            // Act
            var result = _productController.UpdateProduct(updatedProductDTO) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.Equal("Successfully updated", result.Value);
        }

        [Fact]
        public void UpdateProduct_WithInvalidInput_ReturnsBadRequestResult()
        {


            // Act
            var result = _productController.UpdateProduct(null) as BadRequestResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        }

        [Fact]
        public void DeleteProduct_WithValidId_ReturnsNoContentResult()
        {
            // Arrange
            var productId = 1;
            _productInterfaceMock.Setup(repo => repo.ProductExists(productId)).Returns(true);
            _productInterfaceMock.Setup(repo => repo.DeleteProduct(productId)).Returns(true);

            // Act
            var result = _productController.DeleteProduct(productId) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.Equal("Product deleted sucesfuly", result.Value);
        }

        [Fact]
        public void DeleteProduct_WithInvalidId_ReturnsNotFoundResult()
        {
            // Arrange
            var productId = 999;
            _productInterfaceMock.Setup(repo => repo.ProductExists(productId)).Returns(false);

            // Act
            var result = _productController.DeleteProduct(productId) as NotFoundResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        }
    }
}
