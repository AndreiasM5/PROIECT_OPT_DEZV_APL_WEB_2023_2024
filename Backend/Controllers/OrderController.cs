using Backend.Dto;
using Backend.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/order")]
    // [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrderController(OrderService orderService)
        {
            _orderService = orderService;
        }
        
        [HttpGet]
        [Route("orders")]
        public IActionResult GetUserOrders()
        {
            string userId = "1";
            List<OrderDto> userOrdersDto = _orderService.GetUserOrders(userId);
            return Ok(userOrdersDto);
        }

        [HttpGet]
        [Route("{orderId}")]
        public IActionResult GetOrder(int orderId)
        {
            OrderDto orderDto = _orderService.GetOrder(orderId);
            return Ok(orderDto);
        }

        [HttpPost]
        public IActionResult CreateOrder([FromBody] OrderDto orderDto)
        {
            orderDto = _orderService.CreateOrder(orderDto);
            return CreatedAtAction(nameof(GetOrder), new { orderDto.OrderId }, orderDto);
        }

        [HttpDelete]
        [Route("{orderId}")]
        public IActionResult DeleteOrder(int orderId)
        {
            _orderService.DeleteOrder(orderId);
            return NoContent();
        }

        [HttpPut]
        [Route("{orderId}")]
        public IActionResult UpdateOrder(int orderId, [FromBody] OrderDto updatedOrderDto)
        {
            OrderDto updatedDto = _orderService.UpdateOrder(orderId, updatedOrderDto);
            return Ok(updatedDto);
        }
    }
}
