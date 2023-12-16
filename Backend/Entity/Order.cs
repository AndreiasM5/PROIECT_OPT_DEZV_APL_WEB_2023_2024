using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Entity;

[Table("order")]
public class Order
{
    [Column("order_id")]
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int OrderId { get; set; }
    
    public decimal TotalAmount { get; set; }

    // Foreign key property
    [ForeignKey("Customer")]
    public int CustomerId { get; set; }

    // Navigation property back to the parent Customer
    // Customer - Order one-to-many relationship
    public Customer Customer { get; set; }

    // Many to many relationship with Product
    public List<Product> Products { get; set;}
}
