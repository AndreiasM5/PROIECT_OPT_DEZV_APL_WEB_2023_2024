using System.Security.Claims;
using Backend.Dto;
using Backend.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Backend.Controllers;
[Authorize, ApiController, Route("api/order")]
public class OrderController(OrderService service) : ControllerBase {
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException();
    [HttpGet("all")] public IActionResult GetUserOrders() => Ok(service.GetUserOrders(UserId));
    [HttpGet("{orderId:int}")] public IActionResult GetOrder(int orderId) => Ok(service.GetOrder(orderId, UserId, User.IsInRole("Admin")));
    [HttpPost] public IActionResult CreateOrder([FromBody] OrderDto dto) {
        var created = service.CreateOrder(dto, UserId); return CreatedAtAction(nameof(GetOrder), new {orderId = created.OrderId}, created);
    }
    [HttpPut("{orderId:int}")] public IActionResult UpdateOrder(int orderId, [FromBody] OrderDto dto) => Ok(service.UpdateOrder(orderId, dto, UserId));
    [HttpDelete("{orderId:int}")] public IActionResult DeleteOrder(int orderId) {service.DeleteOrder(orderId, UserId); return NoContent();}
}
