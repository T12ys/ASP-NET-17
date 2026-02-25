using ASP_09._Swagger_documentation.Data;
using ASP_09._Swagger_documentation.DTOs.CustomerDTOs;
using ASP_09._Swagger_documentation.Models;
using ASP_09._Swagger_documentation.Services.Interfaces;
using ASP_09._Swagger_documentation.Data;
using ASP_09._Swagger_documentation.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ASP_09._Swagger_documentation.Services;

public class CustomerService : ICustomerService
{
    private readonly AppDbContext _context;

    public CustomerService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CustomerResponseDto> CreateAsync(CreateCustomerDto dto)
    {
        var customer = new Customer
        {
            Name = dto.Name,
            Address = dto.Address,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        return MapToResponseDto(customer);
    }


    public async Task<CustomerResponseDto?> GetByIdAsync(int id)
    {
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == id && c.DeletedAt == null);

        if (customer is null) return null;

        return MapToResponseDto(customer);
    }

    public async Task<CustomerResponseDto?> UpdateAsync(int id, UpdateCustomerDto dto)
    {
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == id && c.DeletedAt == null);

        if (customer == null)
            return null;

        customer.Name = dto.Name;
        customer.Address = dto.Address;
        customer.Email = dto.Email;
        customer.PhoneNumber = dto.PhoneNumber;
        customer.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync();

        return MapToResponseDto(customer);
    }


    public async Task<bool> DeleteAsync(int id)
    {
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == id);

        if (customer == null)
            return false;

        var hasSentInvoices = await _context.Invoices
            .AnyAsync(i =>
                i.CustomerId == id &&
                i.Status != InvoiceStatus.Created &&
                i.DeletedAt == null);

        if (hasSentInvoices)
            return false;

        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();

        return true;
    }


    public async Task<bool> ArchiveAsync(int id)
    {
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == id && c.DeletedAt == null);

        if (customer == null)
            return false;

        customer.DeletedAt = DateTimeOffset.UtcNow;
        customer.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }


    public async Task<IEnumerable<CustomerResponseDto>> GetAllAsync()
    {
        var customers = await _context.Customers
            .Where(c => c.DeletedAt == null)
            .ToListAsync();

        return customers.Select(c => MapToResponseDto(c));
    }





    private CustomerResponseDto MapToResponseDto(Customer customer)
    {
        return new CustomerResponseDto
        {
            Id = customer.Id,
            Name = customer.Name,
            Address = customer.Address,
            Email = customer.Email,
            PhoneNumber = customer.PhoneNumber,
            CreatedAt = customer.CreatedAt,
            UpdatedAt = customer.UpdatedAt,
            DeletedAt = customer.DeletedAt
        };
    }


}
