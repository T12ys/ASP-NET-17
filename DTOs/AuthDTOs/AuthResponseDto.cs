namespace ASP_09._Swagger_documentation.DTOs.AuthDTOs;

public class AuthResponseDto
{
    /// <summary>JWT Access Token</summary>
    public string AccessToken { get; set; } = null!;

    /// <summary>Время истечения Access Token</summary>
    public DateTime AccessTokenExpiresAt { get; set; }

    /// <summary>ID пользователя</summary>
    public int UserId { get; set; }

    /// <summary>Email пользователя</summary>
    public string Email { get; set; } = null!;

    /// <summary>Имя пользователя</summary>
    public string Name { get; set; } = null!;

    /// <summary>Refresh Token (также записывается в cookie)</summary>
    public string? RefreshToken { get; set; }
}