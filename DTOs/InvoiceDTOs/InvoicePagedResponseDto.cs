namespace ASP_09._Swagger_documentation.DTOs.InvoiceDTOs;

public class InvoicePagedResponseDto
{
    /// <summary>Список инвойсов на текущей странице</summary>
    public List<InvoiceResponseDto> Items { get; set; } = new();

    /// <summary>Общее количество инвойсов (без пагинации)</summary>
    public int TotalCount { get; set; }

    /// <summary>Текущая страница</summary>
    public int Page { get; set; }

    /// <summary>Размер страницы</summary>
    public int PageSize { get; set; }

    /// <summary>Общее количество страниц</summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}