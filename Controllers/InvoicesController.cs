// Controllers/InvoicesController.cs
using ASP_09._Swagger_documentation.DTOs.InvoiceDTOs;
using ASP_09._Swagger_documentation.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ASP_09._Swagger_documentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvoicesController : ControllerBase
{
    private readonly IInvoiceService _invoiceService;

    public InvoicesController(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    [HttpGet]
    public async Task<ActionResult<InvoicePagedResponseDto>> GetAll([FromQuery] GetInvoicesQueryDto query)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _invoiceService.GetPagedAsync(query);
        return Ok(result);
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<InvoiceResponseDto>> GetById(int id)
    {
        var invoice = await _invoiceService.GetByIdAsync(id);
        if (invoice is null)
            return NotFound($"Invoice with ID {id} not found");
        return Ok(invoice);
    }

    [HttpPost]
    public async Task<ActionResult<InvoiceResponseDto>> Create([FromBody] CreateInvoiceDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _invoiceService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<InvoiceResponseDto>> Update(int id, [FromBody] UpdateInvoiceDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _invoiceService.UpdateAsync(id, dto);
        if (result is null)
            return BadRequest($"Invoice with ID {id} not found or cannot be edited (already sent)");

        return Ok(result);
    }

    [HttpPatch("{id}/status")]
    public async Task<ActionResult<InvoiceResponseDto>> UpdateStatus(int id, [FromBody] UpdateInvoiceStatusDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _invoiceService.UpdateStatusAsync(id, dto.Status);
        if (result is null)
            return NotFound($"Invoice with ID {id} not found");

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _invoiceService.DeleteAsync(id);
        if (!result)
            return BadRequest($"Invoice with ID {id} not found or cannot be deleted (already sent)");

        return NoContent();
    }

    [HttpPatch("{id}/archive")]
    public async Task<IActionResult> Archive(int id)
    {
        var result = await _invoiceService.ArchiveAsync(id);
        if (!result)
            return NotFound($"Invoice with ID {id} not found or already archived");

        return NoContent();
    }
}