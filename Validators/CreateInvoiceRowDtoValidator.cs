using ASP_09._Swagger_documentation.DTOs.InvoiceDTOs;
using FluentValidation;

namespace ASP_09._Swagger_documentation.Validators;

public class CreateInvoiceRowDtoValidator : AbstractValidator<CreateInvoiceRowDto>
{
    public CreateInvoiceRowDtoValidator()
    {
        RuleFor(x => x.Service)
            .NotEmpty().WithMessage("Название работы обязательно")
            .MaximumLength(500).WithMessage("Название работы не должно превышать 500 символов");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Количество должно быть больше 0");

        RuleFor(x => x.Rate)
            .GreaterThanOrEqualTo(0).WithMessage("Стоимость единицы не может быть отрицательной");
    }
}