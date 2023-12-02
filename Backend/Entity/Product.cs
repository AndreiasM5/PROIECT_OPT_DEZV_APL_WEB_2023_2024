using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Entity;

[Table("product")]
public class Product
{
    [Column("product_id")]
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ProductId { get; set; }

    [Column("name")]
    public string Name { get; set; }
    
    [Column("price")]
    public decimal Price { get; set; }
}