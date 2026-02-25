// Services/Interfaces/IInvoiceService.cs
using ASP_09._Swagger_documentation.DTOs.InvoiceDTOs;
using ASP_09._Swagger_documentation.Models;

namespace ASP_09._Swagger_documentation.Services.Interfaces;

public interface IInvoiceService
{
    Task<InvoiceResponseDto> CreateAsync(CreateInvoiceDto dto);
    Task<InvoiceResponseDto?> GetByIdAsync(int id);
    Task<IEnumerable<InvoiceResponseDto>> GetAllAsync();
    Task<InvoiceResponseDto?> UpdateAsync(int id, UpdateInvoiceDto dto);
    Task<InvoiceResponseDto?> UpdateStatusAsync(int id, InvoiceStatus status);
    Task<bool> DeleteAsync(int id);
    Task<bool> ArchiveAsync(int id);
}