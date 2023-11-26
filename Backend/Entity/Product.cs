using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Entity;

[Table("product")]
public class Product
{
    [Column("product_id")]
    public int ProductId { get; set; }

    [Column("name")]
    public string Name { get; set; }
    
    [Column("price")]
    public decimal Price { get; set; }
}