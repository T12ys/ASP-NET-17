// DTOs/InvoiceDTOs/UpdateInvoiceDto.cs
using System.ComponentModel.DataAnnotations;

namespace ASP_09._Swagger_documentation.DTOs.InvoiceDTOs;

/// <summary>Данные для редактирования инвойса</summary>
public class UpdateInvoiceDto
{
    /// <summary>Начало периода работ</summary>
    public DateTimeOffset StartDate { get; set; }

    /// <summary>Конец периода работ</summary>
    public DateTimeOffset EndDate { get; set; }

    /// <summary>Строки инвойса</summary>
    public List<CreateInvoiceRowDto> Rows { get; set; } = new();

    /// <summary>Комментарий</summary>
    public string? Comment { get; set; }
}