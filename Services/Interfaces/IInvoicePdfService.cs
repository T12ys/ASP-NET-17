namespace ASP_09._Swagger_documentation.Services.Interfaces;

public interface IInvoicePdfService
{
    Task<byte[]?> GenerateAsync(int userId, int invoiceId);
}