using ASP_09._Swagger_documentation.DTOs.InvoiceDTOs;
using ASP_09._Swagger_documentation.Models;

namespace ASP_09._Swagger_documentation.Services.Interfaces;

public interface IInvoiceService
{
    Task<InvoiceResponseDto> CreateAsync(int userId, CreateInvoiceDto dto);
    Task<InvoiceResponseDto?> GetByIdAsync(int userId, int id);
    Task<InvoicePagedResponseDto> GetPagedAsync(int userId, GetInvoicesQueryDto query);
    Task<InvoiceResponseDto?> UpdateAsync(int userId, int id, UpdateInvoiceDto dto);
    Task<InvoiceResponseDto?> UpdateStatusAsync(int userId, int id, InvoiceStatus status);
    Task<bool> DeleteAsync(int userId, int id);
    Task<bool> ArchiveAsync(int userId, int id);
}