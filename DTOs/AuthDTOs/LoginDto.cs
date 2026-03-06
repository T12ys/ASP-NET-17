namespace ASP_09._Swagger_documentation.DTOs.AuthDTOs;

public class LoginDto
{
    /// <summary>Email</summary>
    public string Email { get; set; } = null!;

    /// <summary>Пароль</summary>
    public string Password { get; set; } = null!;
}