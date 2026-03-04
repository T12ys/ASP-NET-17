namespace ASP_09._Swagger_documentation.DTOs.CustomerDTOs;

public class GetCustomersQueryDto
{
    /// <summary>Номер страницы (начиная с 1)</summary>
    public int Page { get; set; } = 1;

    /// <summary>Количество элементов на странице (1-100)</summary>
    public int PageSize { get; set; } = 10;

    /// <summary>Фильтр по имени (частичное совпадение)</summary>
    public string? Name { get; set; }

    /// <summary>Фильтр по email (частичное совпадение)</summary>
    public string? Email { get; set; }

    /// <summary>Включить архивированных клиентов</summary>
    public bool IncludeArchived { get; set; } = false;

    /// <summary>Поле сортировки: name, email, createdAt, updatedAt</summary>
    public string SortBy { get; set; } = "createdAt";

    /// <summary>Направление сортировки: asc, desc</summary>
    public string SortDirection { get; set; } = "asc";
}