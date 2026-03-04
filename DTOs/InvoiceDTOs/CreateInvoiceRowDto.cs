using System.ComponentModel.DataAnnotations;

namespace ASP_09._Swagger_documentation.DTOs.InvoiceDTOs;

public class CreateInvoiceRowDto
{
    /// <summary>Название работы</summary>
    [Required]
    public string Service { get; set; } = null!;

    /// <summary>Количество единиц</summary>
    [Range(0.01, double.MaxValue)]
    public decimal Quantity { get; set; }

    /// <summary>Стоимость единицы</summary>
    [Range(0, double.MaxValue)]
    public decimal Rate { get; set; }
}