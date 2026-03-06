using ASP_09._Swagger_documentation.DTOs.CustomerDTOs;

namespace ASP_09._Swagger_documentation.Services.Interfaces;

public interface ICustomerService
{
    Task<CustomerResponseDto> CreateAsync(int userId, CreateCustomerDto dto);
    Task<CustomerResponseDto?> GetByIdAsync(int userId, int id);
    Task<CustomerResponseDto?> UpdateAsync(int userId, int id, UpdateCustomerDto dto);
    Task<bool> DeleteAsync(int userId, int id);
    Task<bool> ArchiveAsync(int userId, int id);
    Task<CustomerPagedResponseDto> GetPagedAsync(int userId, GetCustomersQueryDto query);
}