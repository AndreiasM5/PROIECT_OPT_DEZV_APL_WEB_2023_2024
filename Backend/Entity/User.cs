using System.ComponentModel.DataAnnotations.Schema;
using Backend.Entity;
using Microsoft.AspNetCore.Identity;

public class User : IdentityUser

{    
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    
    [Column("street")]
    public string? Street { get; set; }
    
    [Column("city")]
    public string? City { get; set; }

    [Column("phone")]
    public string? Phone { get; set; }
}
