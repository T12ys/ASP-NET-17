using ASP_09._Swagger_documentation.Models;

namespace ASP_09._Swagger_documentation.DTOs.InvoiceDTOs;

public class GetInvoicesQueryDto
{
    /// <summary>Номер страницы (начиная с 1)</summary>
    public int Page { get; set; } = 1;

    /// <summary>Количество элементов на странице (1-100)</summary>
    public int PageSize { get; set; } = 10;

    /// <summary>Фильтр по ID клиента</summary>
    public int? CustomerId { get; set; }

    /// <summary>Фильтр по статусу инвойса</summary>
    public InvoiceStatus? Status { get; set; }

    /// <summary>Фильтр: начало периода (StartDate >= значения)</summary>
    public DateTimeOffset? DateFrom { get; set; }

    /// <summary>Фильтр: конец периода (EndDate &lt;= значения)</summary>
    public DateTimeOffset? DateTo { get; set; }

    /// <summary>Включить архивированные инвойсы</summary>
    public bool IncludeArchived { get; set; } = false;

    /// <summary>Поле сортировки: createdAt, updatedAt, startDate, endDate, totalSum, status</summary>
    public string SortBy { get; set; } = "createdAt";

    /// <summary>Направление сортировки: asc, desc</summary>
    public string SortDirection { get; set; } = "asc";
}