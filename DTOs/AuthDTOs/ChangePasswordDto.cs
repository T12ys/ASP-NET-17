namespace ASP_09._Swagger_documentation.DTOs.AuthDTOs;

public class ChangePasswordDto
{
    /// <summary>Текущий пароль</summary>
    public string CurrentPassword { get; set; } = null!;

    /// <summary>Новый пароль</summary>
    public string NewPassword { get; set; } = null!;

    /// <summary>Подтверждение нового пароля</summary>
    public string ConfirmNewPassword { get; set; } = null!;
}