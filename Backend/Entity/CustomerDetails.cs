using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Entity;

[Table("customer_details")]
public class CustomerDetails
{
    [Column("customer_details_id")]
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CustomerDetailsId { get; set; }
    
    [Column("street")]
    public string Street { get; set; }
    
    [Column("city")]
    public string City { get; set; }

    [Column("phone")]
    public string Phone { get; set; }
}