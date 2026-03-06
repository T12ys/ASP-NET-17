namespace ASP_09._Swagger_documentation.DTOs.AuthDTOs;

public class UpdateProfileDto
{
    /// <summary>Имя</summary>
    public string Name { get; set; } = null!;

    /// <summary>Адрес</summary>
    public string? Address { get; set; }

    /// <summary>Номер телефона</summary>
    public string? PhoneNumber { get; set; }
}