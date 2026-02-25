using System.ComponentModel.DataAnnotations;

namespace ASP_09._Swagger_documentation.DTOs.CustomerDTOs;


public class CreateCustomerDto
{
    
    [Required]
    public string Name { get; set; } = null!;

    public string? Address { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    public string? PhoneNumber { get; set; }
}





