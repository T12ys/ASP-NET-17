using ASP_09._Swagger_documentation.DTOs.CustomerDTOs;
using FluentValidation;

namespace ASP_09._Swagger_documentation.Validators;

public class CreateCustomerDtoValidator : AbstractValidator<CreateCustomerDto>
{
    public CreateCustomerDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Имя обязательно")
            .MaximumLength(200).WithMessage("Имя не должно превышать 200 символов");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email обязателен")
            .EmailAddress().WithMessage("Некорректный формат Email")
            .MaximumLength(300).WithMessage("Email не должен превышать 300 символов");

        RuleFor(x => x.Address)
            .MaximumLength(500).WithMessage("Адрес не должен превышать 500 символов")
            .When(x => x.Address is not null);

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20).WithMessage("Телефон не должен превышать 20 символов")
            .Matches(@"^\+?[0-9\s\-\(\)]+$").WithMessage("Некорректный формат номера телефона")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));
    }
}