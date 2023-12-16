using System.ComponentModel.DataAnnotations;

namespace Backend.Dto;

public class CustomerDetailsDTO
{
    public int CustomerDetailsId { get; set; }

    [Required(ErrorMessage = "Please provide street")]
    public string Street { get; set; }

    [Required(ErrorMessage = "Please provide city")]
    public string City { get; set; }

    [Required(ErrorMessage = "Please provide phone number")]
    public string Phone { get; set; }
    
}