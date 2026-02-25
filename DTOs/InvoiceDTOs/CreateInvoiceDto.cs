// DTOs/InvoiceDTOs/CreateInvoiceDto.cs
using System.ComponentModel.DataAnnotations;

namespace ASP_09._Swagger_documentation.DTOs.InvoiceDTOs;

/// <summary>Данные для создания инвойса</summary>
public class CreateInvoiceDto
{
    /// <summary>ID клиента</summary>
    [Required]
    public int CustomerId { get; set; }

    /// <summary>Начало периода работ</summary>
    [Required]
    public DateTimeOffset StartDate { get; set; }

    /// <summary>Конец периода работ</summary>
    [Required]
    public DateTimeOffset EndDate { get; set; }

    /// <summary>Строки инвойса</summary>
    [Required, MinLength(1)]
    public List<CreateInvoiceRowDto> Rows { get; set; } = new();

    /// <summary>Комментарий</summary>
    public string? Comment { get; set; }
}