// DTOs/InvoiceDTOs/UpdateInvoiceStatusDto.cs
using System.ComponentModel.DataAnnotations;
using ASP_09._Swagger_documentation.Models;

namespace ASP_09._Swagger_documentation.DTOs.InvoiceDTOs;

/// <summary>Смена статуса инвойса</summary>
public class UpdateInvoiceStatusDto
{
    
    [Required]
    public InvoiceStatus Status { get; set; }
}