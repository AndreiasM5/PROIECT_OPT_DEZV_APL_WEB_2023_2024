using Backend.Entity;
namespace Backend.Dto;

public class OrderDto
{
    public int OrderId { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal Price { get; set; }
    public string UserId { get; set; }
    public List<int> ProductsIds { get; set; }
}
