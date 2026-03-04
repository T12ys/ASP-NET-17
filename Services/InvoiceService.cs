using ASP_09._Swagger_documentation.Data;
using ASP_09._Swagger_documentation.DTOs.InvoiceDTOs;
using ASP_09._Swagger_documentation.Models;
using ASP_09._Swagger_documentation.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ASP_09._Swagger_documentation.Services;

public class InvoiceService : IInvoiceService
{
    private readonly AppDbContext _context;

    public InvoiceService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<InvoiceResponseDto> CreateAsync(CreateInvoiceDto dto)
    {
        var rows = dto.Rows.Select(r => new InvoiceRow
        {
            Service = r.Service,
            Quantity = r.Quantity,
            Rate = r.Rate,
            Sum = r.Quantity * r.Rate
        }).ToList();

        var invoice = new Invoice
        {
            CustomerId = dto.CustomerId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Comment = dto.Comment,
            Status = InvoiceStatus.Created,
            Rows = rows,
            TotalSum = rows.Sum(r => r.Sum),
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync();

        return MapToResponseDto(invoice);
    }

    public async Task<InvoiceResponseDto?> GetByIdAsync(int id)
    {
        var invoice = await _context.Invoices
            .Include(i => i.Rows)
            .FirstOrDefaultAsync(i => i.Id == id && i.DeletedAt == null);

        return invoice is null ? null : MapToResponseDto(invoice);
    }

    public async Task<IEnumerable<InvoiceResponseDto>> GetAllAsync()
    {
        var invoices = await _context.Invoices
            .Include(i => i.Rows)
            .Where(i => i.DeletedAt == null)
            .ToListAsync();

        return invoices.Select(MapToResponseDto);
    }


    public async Task<InvoicePagedResponseDto> GetPagedAsync(GetInvoicesQueryDto query)
    {
        var q = _context.Invoices
            .Include(i => i.Rows)
            .AsQueryable();

        if (!query.IncludeArchived)
            q = q.Where(i => i.DeletedAt == null);

        if (query.CustomerId.HasValue)
            q = q.Where(i => i.CustomerId == query.CustomerId.Value);

        if (query.Status.HasValue)
            q = q.Where(i => i.Status == query.Status.Value);

        if (query.DateFrom.HasValue)
            q = q.Where(i => i.StartDate >= query.DateFrom.Value);

        if (query.DateTo.HasValue)
            q = q.Where(i => i.EndDate <= query.DateTo.Value);

        q = query.SortBy.ToLower() switch
        {
            "startdate" => query.SortDirection.ToLower() == "desc" ? q.OrderByDescending(i => i.StartDate) : q.OrderBy(i => i.StartDate),
            "enddate" => query.SortDirection.ToLower() == "desc" ? q.OrderByDescending(i => i.EndDate) : q.OrderBy(i => i.EndDate),
            "totalsum" => query.SortDirection.ToLower() == "desc" ? q.OrderByDescending(i => i.TotalSum) : q.OrderBy(i => i.TotalSum),
            "status" => query.SortDirection.ToLower() == "desc" ? q.OrderByDescending(i => i.Status) : q.OrderBy(i => i.Status),
            "updatedat" => query.SortDirection.ToLower() == "desc" ? q.OrderByDescending(i => i.UpdatedAt) : q.OrderBy(i => i.UpdatedAt),
            _ => query.SortDirection.ToLower() == "desc" ? q.OrderByDescending(i => i.CreatedAt) : q.OrderBy(i => i.CreatedAt),
        };

        var totalCount = await q.CountAsync();

        var items = await q
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return new InvoicePagedResponseDto
        {
            Items = items.Select(MapToResponseDto).ToList(),
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }


    public async Task<InvoiceResponseDto?> UpdateAsync(int id, UpdateInvoiceDto dto)
    {
        var invoice = await _context.Invoices
            .Include(i => i.Rows)
            .FirstOrDefaultAsync(i => i.Id == id && i.DeletedAt == null);

        if (invoice is null) return null;

        if (invoice.Status != InvoiceStatus.Created) return null;

        _context.InvoiceRows.RemoveRange(invoice.Rows);

        var newRows = dto.Rows.Select(r => new InvoiceRow
        {
            InvoiceId = id,
            Service = r.Service,
            Quantity = r.Quantity,
            Rate = r.Rate,
            Sum = r.Quantity * r.Rate
        }).ToList();

        invoice.StartDate = dto.StartDate;
        invoice.EndDate = dto.EndDate;
        invoice.Comment = dto.Comment;
        invoice.Rows = newRows;
        invoice.TotalSum = newRows.Sum(r => r.Sum);
        invoice.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync();

        return MapToResponseDto(invoice);
    }

    public async Task<InvoiceResponseDto?> UpdateStatusAsync(int id, InvoiceStatus status)
    {
        var invoice = await _context.Invoices
            .Include(i => i.Rows)
            .FirstOrDefaultAsync(i => i.Id == id && i.DeletedAt == null);

        if (invoice is null) return null;

        invoice.Status = status;
        invoice.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync();

        return MapToResponseDto(invoice);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var invoice = await _context.Invoices
            .FirstOrDefaultAsync(i => i.Id == id);

        if (invoice is null) return false;

        if (invoice.Status != InvoiceStatus.Created) return false;

        _context.Invoices.Remove(invoice);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ArchiveAsync(int id)
    {
        var invoice = await _context.Invoices
            .FirstOrDefaultAsync(i => i.Id == id && i.DeletedAt == null);

        if (invoice is null) return false;

        invoice.DeletedAt = DateTimeOffset.UtcNow;
        invoice.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }

    private static InvoiceResponseDto MapToResponseDto(Invoice invoice)
    {
        return new InvoiceResponseDto
        {
            Id = invoice.Id,
            CustomerId = invoice.CustomerId,
            StartDate = invoice.StartDate,
            EndDate = invoice.EndDate,
            TotalSum = invoice.TotalSum,
            Comment = invoice.Comment,
            Status = invoice.Status,
            CreatedAt = invoice.CreatedAt,
            UpdatedAt = invoice.UpdatedAt,
            DeletedAt = invoice.DeletedAt,
            Rows = invoice.Rows.Select(r => new InvoiceRowResponseDto
            {
                Id = r.Id,
                Service = r.Service,
                Quantity = r.Quantity,
                Rate = r.Rate,
                Sum = r.Sum
            }).ToList()
        };
    }
}