using ASP_09._Swagger_documentation.DTOs.AuthDTOs;
using FluentValidation;

namespace ASP_09._Swagger_documentation.Validators;

public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Имя обязательно")
            .MaximumLength(200).WithMessage("Имя не должно превышать 200 символов");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email обязателен")
            .EmailAddress().WithMessage("Некорректный формат Email")
            .MaximumLength(200).WithMessage("Email не должен превышать 200 символов");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Пароль обязателен")
            .MinimumLength(6).WithMessage("Пароль должен содержать минимум 6 символов")
            .MaximumLength(100).WithMessage("Пароль не должен превышать 100 символов");

        RuleFor(x => x.Address)
            .MaximumLength(500).WithMessage("Адрес не должен превышать 500 символов")
            .When(x => x.Address is not null);

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(50).WithMessage("Телефон не должен превышать 50 символов")
            .Matches(@"^\+?[0-9\s\-\(\)]+$").WithMessage("Некорректный формат номера телефона")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));
    }
}