using ASP_09._Swagger_documentation.DTOs.InvoiceDTOs;
using FluentValidation;

namespace ASP_09._Swagger_documentation.Validators;

public class CreateInvoiceDtoValidator : AbstractValidator<CreateInvoiceDto>
{
    public CreateInvoiceDtoValidator()
    {
        RuleFor(x => x.CustomerId)
            .GreaterThan(0).WithMessage("CustomerId должен быть больше 0");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Дата начала обязательна");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("Дата окончания обязательна")
            .GreaterThan(x => x.StartDate).WithMessage("Дата окончания должна быть позже даты начала");

        RuleFor(x => x.Rows)
            .NotEmpty().WithMessage("Инвойс должен содержать хотя бы одну строку");

        RuleForEach(x => x.Rows).SetValidator(new CreateInvoiceRowDtoValidator());

        RuleFor(x => x.Comment)
            .MaximumLength(1000).WithMessage("Комментарий не должен превышать 1000 символов")
            .When(x => x.Comment is not null);
    }
}