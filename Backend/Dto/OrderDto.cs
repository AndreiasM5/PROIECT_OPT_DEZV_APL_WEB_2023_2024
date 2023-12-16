using Backend.Entity;

namespace Backend.Dto;


public class OrderDto
{
    public int OrderId { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal Price { get; set; }
    public int CustomerId { get; set; }
    public Customer Customer { get; set; }
    public List<ProductDto> Products { get; set;}
}
