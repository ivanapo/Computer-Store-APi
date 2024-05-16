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
using System.Linq;
using Xunit;

namespace ComputerStore.Tests.Controllers
{
    public class CategoryControllerTests
    {
        private readonly CategoryController _categoryController;
        private readonly Mock<ICategoryInterface> _categoryInterfaceMock;
        private readonly Mock<IMapper> _mapperMock;

        public CategoryControllerTests()
        {
            _categoryInterfaceMock = new Mock<ICategoryInterface>();
            _mapperMock = new Mock<IMapper>();
            _categoryController = new CategoryController(_categoryInterfaceMock.Object, _mapperMock.Object);
        }

        [Fact]
        public void GetCategories_ReturnsOkResult()
        {
            // Arrange
            var category = new Category { CategoryId = 1, Name = "Sample Category", Description = "Sample Description" };
            var categories = new List<Category> { category }; 
            var categoryDTO = new CategoryDTO { Id = category.CategoryId, Name = category.Name, Description = category.Description };
            var categoryDTOs = new List<CategoryDTO> { categoryDTO };
            _categoryInterfaceMock.Setup(repo => repo.GetCategories()).Returns(categoryDTOs);
            _mapperMock.Setup(mapper => mapper.Map<ICollection<CategoryDTO>>(categories)).Returns(categoryDTOs);

            // Act
            var result = _categoryController.GetCategories() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.Equal(categoryDTOs, result.Value);
        }


        [Fact]
        public void GetCategory_WithValidId_ReturnsOkResult()
        {
            // Arrange
            var categoryId = 1;
            var category = new Category { CategoryId = 1, Name = "Sample Category", Description = "Sample Description" };
            var categoryDTO = new CategoryDTO { Id = category.CategoryId, Name = category.Name, Description = category.Description };
            _categoryInterfaceMock.Setup(repo => repo.CategoryExists(categoryId)).Returns(true);
            _categoryInterfaceMock.Setup(repo => repo.GetCategory(categoryId)).Returns(category);
            _mapperMock.Setup(mapper => mapper.Map<CategoryDTO>(category)).Returns(categoryDTO);

            // Act
            var result = _categoryController.GetCategory(categoryId) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.Equal(categoryDTO, result.Value);
        }

        [Fact]
        public void GetCategory_WithInvalidId_ReturnsNotFoundResult()
        {
            // Arrange
            var categoryId = 999;
            _categoryInterfaceMock.Setup(repo => repo.CategoryExists(categoryId)).Returns(false);

            // Act
            var result = _categoryController.GetCategory(categoryId) as NotFoundResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        }

        [Fact]
        public void GetProductByCategoryId_WithValidCategoryId_ReturnsOkResult()
        {
            // Arrange
            var categoryId = 1;
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

            var products = new List<Product> { product }; 

            var productDTO = new ProductDTO
            {
                Name = product.Name,
                Discription = product.Discription,
                Price = product.Price,
                Quantity = product.Quantity,
                ProductCategories = product.ProductCategories.Select(c => c.Name).ToList()
            };
            var productDTOs = new List<ProductDTO> { productDTO }; 

            _categoryInterfaceMock.Setup(repo => repo.GetProductByCategory(categoryId)).Returns(products);
            _mapperMock.Setup(mapper => mapper.Map<IEnumerable<ProductDTO>>(products)).Returns(productDTOs);

            // Act
            var result = _categoryController.GetProductByCategoryId(categoryId) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.Equal(productDTOs, result.Value);
        }


        [Fact]
        public void GetProductByCategoryId_WithInvalidCategoryId_ReturnsNotFoundResult()
        {
            // Arrange
            var categoryId = 999;
            _categoryInterfaceMock.Setup(repo => repo.GetProductByCategory(categoryId)).Returns((ICollection<Product>)null);

            // Act
            var result = _categoryController.GetProductByCategoryId(categoryId) as NotFoundObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
            Assert.Equal("No products found for that category", result.Value);
        }


        [Fact]
        public void CreateCategory_WithValidInput_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var category = new Category { CategoryId = 1, Name = "Sample Category", Description = "Sample Description" };
            var categoryDTO = new CategoryDTO { Id = category.CategoryId, Name = category.Name, Description = category.Description };
            _mapperMock.Setup(mapper => mapper.Map<Category>(categoryDTO)).Returns(category);
            _categoryInterfaceMock.Setup(repo => repo.GetCategories()).Returns(new List<CategoryDTO>());
            _categoryInterfaceMock.Setup(repo => repo.CreateCategory(It.IsAny<Category>())).Returns(true);

            // Act
            var result = _categoryController.CreateCategory(categoryDTO) as CreatedAtActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
            Assert.Equal(nameof(CategoryController.GetCategory), result.ActionName);
        }


        [Fact]
        public void CreateCategory_WithNullInput_ReturnsBadRequestResult()
        {
            // Arrange

            // Act
            var result = _categoryController.CreateCategory(null) as BadRequestResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        }

        [Fact]
        public void UpdateCategory_WithValidInput_ReturnsOkResult()
        {
            // Arrange
            var updatedCategory = new Category { CategoryId = 1, Name = "Sample Category", Description = "Sample Description" };
            var updatedCategoryDTO = new CategoryDTO { Id = updatedCategory.CategoryId, Name = updatedCategory.Name, Description = updatedCategory.Description };
            _categoryInterfaceMock.Setup(repo => repo.CategoryExists(updatedCategoryDTO.Id)).Returns(true);
            _mapperMock.Setup(mapper => mapper.Map<Category>(updatedCategoryDTO)).Returns(updatedCategory);
            _categoryInterfaceMock.Setup(repo => repo.UpdateCategory(It.IsAny<Category>())).Returns(true);

            // Act
            var result = _categoryController.UpdateCategory(updatedCategoryDTO) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.Equal("Update Successfully", result.Value);
        }

        [Fact]
        public void UpdateCategory_WithInvalidInput_ReturnsBadRequestResult()
        {
            // Arrange

            // Act
            var result = _categoryController.UpdateCategory(null) as BadRequestResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        }

        [Fact]
        public void DeleteCategory_WithValidId_ReturnsOkResult()
        {
            // Arrange
            var categoryId = 1;
            _categoryInterfaceMock.Setup(repo => repo.CategoryExists(categoryId)).Returns(true);
            _categoryInterfaceMock.Setup(repo => repo.DeleteCategory(categoryId)).Returns(true);

            // Act
            var result = _categoryController.DeleteCategory(categoryId) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.Equal("Successfully deleted", result.Value);
        }

        [Fact]
        public void DeleteCategory_WithInvalidId_ReturnsNotFoundResult()
        {
            // Arrange
            var categoryId = 999;
            _categoryInterfaceMock.Setup(repo => repo.CategoryExists(categoryId)).Returns(false);

            // Act
            var result = _categoryController.DeleteCategory(categoryId) as NotFoundObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
            Assert.Equal("Category not found", result.Value);
        }
    }
}
