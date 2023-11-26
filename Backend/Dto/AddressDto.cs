using System.ComponentModel.DataAnnotations;

namespace Backend.Dto;

public class AddressDTO
{
    public int AddressId { get; set; }

    [Required(ErrorMessage = "Please provide street")]
    public string Street { get; set; }

    [Required(ErrorMessage = "Please provide city")]
    public string City { get; set; }
    
}