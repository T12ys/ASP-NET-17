using System.Security.Claims;
using ASP_09._Swagger_documentation.DTOs.InvoiceDTOs;
using ASP_09._Swagger_documentation.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASP_09._Swagger_documentation.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class InvoicesController : ControllerBase
{
    private readonly IInvoiceService _invoiceService;
    private readonly IInvoicePdfService _invoicePdfService;

    public InvoicesController(IInvoiceService invoiceService, IInvoicePdfService invoicePdfService)
    {
        _invoiceService = invoiceService;
        _invoicePdfService = invoicePdfService;
    }

    /// <summary>Получить список инвойсов с пагинацией, фильтрацией и сортировкой</summary>
    [HttpGet]
    public async Task<ActionResult<InvoicePagedResponseDto>> GetAll([FromQuery] GetInvoicesQueryDto query)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _invoiceService.GetPagedAsync(GetUserId(), query);
        return Ok(result);
    }

    /// <summary>Получить инвойс по ID</summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<InvoiceResponseDto>> GetById(int id)
    {
        var invoice = await _invoiceService.GetByIdAsync(GetUserId(), id);
        if (invoice is null) return NotFound($"Invoice with ID {id} not found");
        return Ok(invoice);
    }

    /// <summary>Скачать инвойс в формате PDF</summary>
    [HttpGet("{id}/download")]
    public async Task<IActionResult> Download(int id)
    {
        var pdfBytes = await _invoicePdfService.GenerateAsync(GetUserId(), id);

        if (pdfBytes is null)
            return NotFound($"Invoice with ID {id} not found");

        return File(pdfBytes, "application/pdf", $"invoice-{id}.pdf");
    }

    /// <summary>Создать инвойс</summary>
    [HttpPost]
    public async Task<ActionResult<InvoiceResponseDto>> Create([FromBody] CreateInvoiceDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var result = await _invoiceService.CreateAsync(GetUserId(), dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>Обновить инвойс (только Created)</summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<InvoiceResponseDto>> Update(int id, [FromBody] UpdateInvoiceDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _invoiceService.UpdateAsync(GetUserId(), id, dto);
        if (result is null) return BadRequest($"Invoice with ID {id} not found or cannot be edited (already sent)");
        return Ok(result);
    }

    /// <summary>Изменить статус инвойса</summary>
    [HttpPatch("{id}/status")]
    public async Task<ActionResult<InvoiceResponseDto>> UpdateStatus(int id, [FromBody] UpdateInvoiceStatusDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _invoiceService.UpdateStatusAsync(GetUserId(), id, dto.Status);
        if (result is null) return NotFound($"Invoice with ID {id} not found");
        return Ok(result);
    }

    /// <summary>Удалить инвойс (hard delete, только Created)</summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _invoiceService.DeleteAsync(GetUserId(), id);
        if (!result) return BadRequest($"Invoice with ID {id} not found or cannot be deleted (already sent)");
        return NoContent();
    }

    /// <summary>Архивировать инвойс (soft delete)</summary>
    [HttpPatch("{id}/archive")]
    public async Task<IActionResult> Archive(int id)
    {
        var result = await _invoiceService.ArchiveAsync(GetUserId(), id);
        if (!result) return NotFound($"Invoice with ID {id} not found or already archived");
        return NoContent();
    }

    private int GetUserId() =>
        int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
}