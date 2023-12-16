using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend.Entity;

[Table("customer")]
public class Customer
{
    [Column("customer_id")]
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CustomerId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

    // Navigation property for one-to-one relationship
    public CustomerDetails CustomerDetails { get; set; }

}