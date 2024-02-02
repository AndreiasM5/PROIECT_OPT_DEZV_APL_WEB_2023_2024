using System.Security.Claims;
using Backend.Dto;
using Backend.Entity;
using Backend.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[Authorize]
[ApiController]
[Route("api/order")]
public class OrderController : ControllerBase
{
    private readonly OrderService _orderService;
    private readonly UserManager<User> _userManager;

    public OrderController(OrderService orderService, UserManager<User> userManager)
    {
        _orderService = orderService;
        _userManager = userManager;
    }

    [HttpGet]
    [Route("all")]
    public async Task<IActionResult> GetUserOrders()
    {
    var userEmail = User.FindFirstValue(ClaimTypes.Name);
    User user = await _userManager.FindByNameAsync(userEmail);  

    if (user == null)
    {
        return BadRequest("User not found");
    }
        List<OrderDto> userOrdersDto = _orderService.GetUserOrders(user.Id);
        return Ok(userOrdersDto);
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrderAsync([FromBody] OrderDto orderDto)
    {   
        var userEmail = User.FindFirstValue(ClaimTypes.Name);
        User user = await _userManager.FindByNameAsync(userEmail);  
        if (user == null)
        {
            return BadRequest("User not found");
        }

        orderDto = _orderService.CreateOrder(orderDto, user.Id);
        return CreatedAtAction(nameof(GetOrder), new { orderDto.OrderId }, orderDto);
    }

    [HttpDelete]
    [Route("{orderId}")]
    public async Task<IActionResult> DeleteOrderAsync(int orderId)
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Name);
        User user = await _userManager.FindByNameAsync(userEmail);  

        if (user == null)
        {
            return BadRequest("User not found");
        }
        
        _orderService.DeleteOrder(orderId, user.Id);
        return NoContent();
    }

    [HttpPut]
    [Route("{orderId}")]
    public async Task<IActionResult> UpdateOrderAsync(int orderId, [FromBody] OrderDto updatedOrderDto)
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Name);
        User user = await _userManager.FindByNameAsync(userEmail);  

        if (user == null)
        {
            return BadRequest("User not found");
        }

        OrderDto updatedDto = _orderService.UpdateOrder(orderId, updatedOrderDto, user.Id);
        return Ok(updatedDto);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    [Route("{orderId}")]
    public IActionResult GetOrder(int orderId)
    {
        OrderDto orderDto = _orderService.GetOrder(orderId);
        return Ok(orderDto);
    }
}

