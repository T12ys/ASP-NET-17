using System.ComponentModel.DataAnnotations;
using ASP_09._Swagger_documentation.Models;

namespace ASP_09._Swagger_documentation.DTOs.InvoiceDTOs;

public class UpdateInvoiceStatusDto
{
    
    [Required]
    public InvoiceStatus Status { get; set; }
}