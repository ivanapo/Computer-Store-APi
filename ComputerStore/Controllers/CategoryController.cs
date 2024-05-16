using AutoMapper;
using ComputerStore.DTO;
using ComputerStore.Interfaces;
using ComputerStore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace ComputerStore.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryInterface _iCategoryInterface;
        private readonly IMapper _mapper;

        public CategoryController(ICategoryInterface categoryInterface, IMapper mapper)
        {
            _iCategoryInterface = categoryInterface;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CategoryDTO>))]
        public IActionResult GetCategories()
        {
            var categories = _iCategoryInterface.GetCategories();
            var categoryDTOs = _mapper.Map<IEnumerable<CategoryDTO>>(categories);
            return Ok(categoryDTOs);
        }


        [HttpGet("{categoryId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CategoryDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetCategory(int categoryId)
        {
            if (!_iCategoryInterface.CategoryExists(categoryId))
                return NotFound();

            var category = _iCategoryInterface.GetCategory(categoryId);
            var categoryDTO = _mapper.Map<CategoryDTO>(category);

            return Ok(categoryDTO);
        }

        [HttpGet("product/{categoryId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ProductDTO>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetProductByCategoryId(int categoryId)
        {
            var products = _iCategoryInterface.GetProductByCategory(categoryId);

            if (products == null || !products.Any())
            {
                return NotFound("No products found for that category");
            }

            var productDTOs = _mapper.Map<IEnumerable<ProductDTO>>(products);

            return Ok(productDTOs);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateCategory([FromBody] CategoryDTO categoryCreate)
        {
            try
            {
                if (categoryCreate == null)
                {
                    return BadRequest();
                }

                var existingCategory = _iCategoryInterface.GetCategories()
                    .FirstOrDefault(c => c.Name.Trim().ToUpper() == categoryCreate.Name.Trim().ToUpper());

                if (existingCategory != null)
                {
                    return UnprocessableEntity("Category already exists");
                }

                var category = _mapper.Map<Category>(categoryCreate);

                if (!_iCategoryInterface.CreateCategory(category))
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Failed to save category");
                }

                return CreatedAtAction(nameof(GetCategory), new { categoryId = category.CategoryId }, category);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateCategory([FromBody] CategoryDTO updatedCategoryDTO)
        {
            try
            {
                if (updatedCategoryDTO == null || !_iCategoryInterface.CategoryExists(updatedCategoryDTO.Id))
                {
                    return BadRequest();
                }

                var categoryToUpdate = _mapper.Map<Category>(updatedCategoryDTO);
                if (!_iCategoryInterface.UpdateCategory(categoryToUpdate))
                {
                    ModelState.AddModelError("", "Something went wrong while updating the category.");
                    return StatusCode(500, ModelState);
                }

                return Ok("Update Successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteCategory(int id)
        {
            try
            {
                if (!_iCategoryInterface.CategoryExists(id))
                {
                    return NotFound("Category not found");
                }

                if (!_iCategoryInterface.DeleteCategory(id))
                {
                    ModelState.AddModelError("", "Something went wrong while deleting the category.");
                    return StatusCode(500, ModelState);
                }

                return Ok("Successfully deleted");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
