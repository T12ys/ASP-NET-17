namespace ASP_09._Swagger_documentation.DTOs.InvoiceDTOs;

public class InvoiceRowResponseDto
{
    public int Id { get; set; }
    public string Service { get; set; } = null!;
    public decimal Quantity { get; set; }
    public decimal Rate { get; set; }
    public decimal Sum { get; set; }
}