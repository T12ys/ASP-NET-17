using ASP_09._Swagger_documentation.DTOs.CustomerDTOs;
using FluentValidation;

namespace ASP_09._Swagger_documentation.Validators;
/// <summary>
/// Валидатор параметров запроса списка клиентов.
/// Проверяет корректность пагинации, сортировки и фильтров.
/// </summary>
public class GetCustomersQueryDtoValidator : AbstractValidator<GetCustomersQueryDto>
{
    private static readonly string[] AllowedSortFields = ["name", "email", "createdat", "updatedat"];
    private static readonly string[] AllowedSortDirections = ["asc", "desc"];

    public GetCustomersQueryDtoValidator()
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

        RuleFor(x => x.Name)
            .MaximumLength(200).WithMessage("Фильтр по имени не должен превышать 200 символов")
            .When(x => x.Name is not null);

        RuleFor(x => x.Email)
            .MaximumLength(300).WithMessage("Фильтр по email не должен превышать 300 символов")
            .When(x => x.Email is not null);
    }
}