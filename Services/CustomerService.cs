using ASP_09._Swagger_documentation.Data;
using ASP_09._Swagger_documentation.DTOs.CustomerDTOs;
using ASP_09._Swagger_documentation.Models;
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

    public async Task<CustomerResponseDto> CreateAsync(int userId, CreateCustomerDto dto)
    {
        var customer = new Customer
        {
            UserId = userId,
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

    public async Task<CustomerResponseDto?> GetByIdAsync(int userId, int id)
    {
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId && c.DeletedAt == null);

        return customer is null ? null : MapToResponseDto(customer);
    }

    public async Task<CustomerResponseDto?> UpdateAsync(int userId, int id, UpdateCustomerDto dto)
    {
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId && c.DeletedAt == null);

        if (customer is null) return null;

        customer.Name = dto.Name;
        customer.Address = dto.Address;
        customer.Email = dto.Email;
        customer.PhoneNumber = dto.PhoneNumber;
        customer.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync();

        return MapToResponseDto(customer);
    }

    public async Task<bool> DeleteAsync(int userId, int id)
    {
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

        if (customer is null) return false;

        var hasSentInvoices = await _context.Invoices
            .AnyAsync(i =>
                i.CustomerId == id &&
                i.Status != InvoiceStatus.Created &&
                i.DeletedAt == null);

        if (hasSentInvoices) return false;

        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ArchiveAsync(int userId, int id)
    {
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId && c.DeletedAt == null);

        if (customer is null) return false;

        customer.DeletedAt = DateTimeOffset.UtcNow;
        customer.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<CustomerPagedResponseDto> GetPagedAsync(int userId, GetCustomersQueryDto query)
    {
        var q = _context.Customers
            .Where(c => c.UserId == userId)
            .AsQueryable();

        if (!query.IncludeArchived)
            q = q.Where(c => c.DeletedAt == null);

        if (!string.IsNullOrWhiteSpace(query.Name))
            q = q.Where(c => c.Name.Contains(query.Name));

        if (!string.IsNullOrWhiteSpace(query.Email))
            q = q.Where(c => c.Email.Contains(query.Email));

        q = query.SortBy.ToLower() switch
        {
            "name" => query.SortDirection.ToLower() == "desc" ? q.OrderByDescending(c => c.Name) : q.OrderBy(c => c.Name),
            "email" => query.SortDirection.ToLower() == "desc" ? q.OrderByDescending(c => c.Email) : q.OrderBy(c => c.Email),
            "updatedat" => query.SortDirection.ToLower() == "desc" ? q.OrderByDescending(c => c.UpdatedAt) : q.OrderBy(c => c.UpdatedAt),
            _ => query.SortDirection.ToLower() == "desc" ? q.OrderByDescending(c => c.CreatedAt) : q.OrderBy(c => c.CreatedAt),
        };

        var totalCount = await q.CountAsync();

        var items = await q
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return new CustomerPagedResponseDto
        {
            Items = items.Select(MapToResponseDto).ToList(),
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    private static CustomerResponseDto MapToResponseDto(Customer c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        Address = c.Address,
        Email = c.Email,
        PhoneNumber = c.PhoneNumber,
        CreatedAt = c.CreatedAt,
        UpdatedAt = c.UpdatedAt,
        DeletedAt = c.DeletedAt
    };
}