using System.ComponentModel.DataAnnotations;
namespace Backend.Dto;
public class OrderDto {
    public int OrderId {get;set;}
    // Returned by the API; client-supplied totals are ignored and recalculated.
    public decimal TotalAmount {get;set;}
    [Required, MinLength(1), MaxLength(100)] public List<int> ProductsIds {get;set;} = [];
}
