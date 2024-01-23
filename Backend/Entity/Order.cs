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
    [ForeignKey("User")]
    public string UserId { get; set; }

    // Navigation property back to the parent User
    // User - Order one-to-many relationship
    public User User { get; set; }

    // Many to many relationship with Product
    public List<Product> Products { get; set;}
}
