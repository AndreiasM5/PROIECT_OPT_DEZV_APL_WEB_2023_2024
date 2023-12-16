using System.ComponentModel.DataAnnotations;
using Backend.Entity;

namespace Backend.Dto;

public class CustomerDTO
{
    public int CustomerId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public CustomerDetails CustomerDetails { get; set; }
    
}