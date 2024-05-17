using AutoMapper;
using ComputerStore.DTO;
using ComputerStore.Interfaces;
using ComputerStore.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace ComputerStore.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductInterface _productInterface;
        private readonly IMapper _mapper;

        public ProductController(IProductInterface productInterface, IMapper mapper)
        {
            _productInterface = productInterface;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Product>))]
        public IActionResult GetProducts()
        {
            var products = _productInterface.GetProducts();

            return Ok(products);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(ProductDTO))]
        [ProducesResponseType(404)]
        public IActionResult GetProduct(int id)
        {
            if (!_productInterface.ProductExists(id))
                return NotFound("Id not found");

            var product = _productInterface.GetProduct(id);
            var productDTO = _mapper.Map<ProductDTO>(product);

            return Ok(productDTO);
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult CreateProduct([FromBody] ProductDTO productCreate)
        {
            if (productCreate == null)
                return BadRequest();

            var product = _mapper.Map<ProductDTO>(productCreate);

            if (!_productInterface.CreateProduct(product))
            {
                return StatusCode(500, "Something went wrong while saving");
            }


            return Ok("Product Created Successfully");
        }
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public IActionResult UpdateProduct([FromBody] UpdateProductDTO updatedProductDTO)
        {
            try
            {
                if (updatedProductDTO == null || !_productInterface.ProductExists(updatedProductDTO.Id))
                {
                    return BadRequest();
                }


                var productToUpdate = new Product
                {
                    ProductId = updatedProductDTO.Id,
                    Name = updatedProductDTO.Name,
                    Discription = updatedProductDTO.Discription,
                    Price = Convert.ToDecimal(updatedProductDTO.Price),
                    Quantity = updatedProductDTO.Quantity,
                    
                };


                if (!_productInterface.UpdateProduct(productToUpdate))
                {
                    return StatusCode(500, "Something went wrong while updating the product.");
                }

                return Ok("Successfully updated");
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
        public IActionResult DeleteProduct(int id)
        {
            try
            {
                if (!_productInterface.ProductExists(id))
                {
                    return NotFound();
                }

                if (!_productInterface.DeleteProduct(id))
                {
                    return StatusCode(500, "Something went wrong while deleting the product.");
                }

                return Ok("Product deleted sucesfuly");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
