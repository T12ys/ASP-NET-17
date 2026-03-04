namespace ASP_09._Swagger_documentation.DTOs.CustomerDTOs;

public class CustomerPagedResponseDto
{
    /// <summary>Список клиентов на текущей странице</summary>
    public List<CustomerResponseDto> Items { get; set; } = new();

    /// <summary>Общее количество клиентов (без пагинации)</summary>
    public int TotalCount { get; set; }

    /// <summary>Текущая страница</summary>
    public int Page { get; set; }

    /// <summary>Размер страницы</summary>
    public int PageSize { get; set; }

    /// <summary>Общее количество страниц</summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}