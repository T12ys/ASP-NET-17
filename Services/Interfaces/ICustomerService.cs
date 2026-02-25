using ASP_09._Swagger_documentation.DTOs.CustomerDTOs;
using Microsoft.EntityFrameworkCore;

namespace ASP_09._Swagger_documentation.Services.Interfaces;

public interface ICustomerService
{
    Task<CustomerResponseDto> CreateAsync(CreateCustomerDto dto);

    Task<CustomerResponseDto?> GetByIdAsync(int id);

    Task<CustomerResponseDto> UpdateAsync(int id, UpdateCustomerDto dto);

    Task<bool> DeleteAsync(int id);

    Task<bool> ArchiveAsync(int id);

    Task<IEnumerable<CustomerResponseDto>> GetAllAsync();

}
