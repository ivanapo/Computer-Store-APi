using ComputerStore.DTO;
using ComputerStore.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ComputerStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrderController(OrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("calculate-discount")]
        public IActionResult CalculateDiscount([FromBody] List<Order> products)
        {
            try
            {
                var discount = _orderService.CalculateDiscount(products);
                return Ok(discount);
            }
            catch (Exception ex)
            {

                return StatusCode(500, "An error occurred while calculating discount.");
            }
        }
    }
}