namespace ASP_09._Swagger_documentation.DTOs.AuthDTOs;

public class RegisterDto
{
    /// <summary>Имя пользователя</summary>
    public string Name { get; set; } = null!;

    /// <summary>Email (используется для входа)</summary>
    public string Email { get; set; } = null!;

    /// <summary>Пароль</summary>
    public string Password { get; set; } = null!;

    /// <summary>Адрес</summary>
    public string? Address { get; set; }

    /// <summary>Номер телефона</summary>
    public string? PhoneNumber { get; set; }
}