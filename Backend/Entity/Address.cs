using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Entity;

[Table("address")]
public class Address
{
    [Column("address_id")]
    public int AddressId { get; set; }
    
    [Column("street")]
    public string Street { get; set; }
    
    [Column("city")]
    public string City { get; set; }
}