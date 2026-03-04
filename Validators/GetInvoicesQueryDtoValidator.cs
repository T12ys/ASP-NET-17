using ASP_09._Swagger_documentation.DTOs.InvoiceDTOs;
using FluentValidation;

namespace ASP_09._Swagger_documentation.Validators;
/// <summary>
/// Валидатор параметров запроса списка инвойсов.
/// Проверяет корректность пагинации, сортировки и фильтров.
/// </summary>
public class GetInvoicesQueryDtoValidator : AbstractValidator<GetInvoicesQueryDto>
{
    private static readonly string[] AllowedSortFields = ["createdat", "updatedat", "startdate", "enddate", "totalsum", "status"];
    private static readonly string[] AllowedSortDirections = ["asc", "desc"];

    public GetInvoicesQueryDtoValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Номер страницы должен быть не меньше 1");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Размер страницы должен быть от 1 до 100");

        RuleFor(x => x.SortBy)
            .Must(v => AllowedSortFields.Contains(v.ToLower()))
            .WithMessage($"Допустимые поля сортировки: {string.Join(", ", AllowedSortFields)}");

        RuleFor(x => x.SortDirection)
            .Must(v => AllowedSortDirections.Contains(v.ToLower()))
            .WithMessage("Направление сортировки должно быть 'asc' или 'desc'");

        RuleFor(x => x.CustomerId)
            .GreaterThan(0).WithMessage("CustomerId должен быть больше 0")
            .When(x => x.CustomerId.HasValue);

        RuleFor(x => x.DateFrom)
            .LessThan(x => x.DateTo).WithMessage("DateFrom должна быть раньше DateTo")
            .When(x => x.DateFrom.HasValue && x.DateTo.HasValue);
    }
}