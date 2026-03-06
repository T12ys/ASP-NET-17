using ASP_09._Swagger_documentation.DTOs.AuthDTOs;
using FluentValidation;

namespace ASP_09._Swagger_documentation.Validators;

public class ChangePasswordDtoValidator : AbstractValidator<ChangePasswordDto>
{
    public ChangePasswordDtoValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("Текущий пароль обязателен");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("Новый пароль обязателен")
            .MinimumLength(6).WithMessage("Пароль должен содержать минимум 6 символов")
            .MaximumLength(100).WithMessage("Пароль не должен превышать 100 символов")
            .NotEqual(x => x.CurrentPassword).WithMessage("Новый пароль должен отличаться от текущего");

        RuleFor(x => x.ConfirmNewPassword)
            .NotEmpty().WithMessage("Подтверждение пароля обязательно")
            .Equal(x => x.NewPassword).WithMessage("Пароли не совпадают");
    }
}