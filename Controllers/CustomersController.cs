using System.Security.Claims;
using ASP_09._Swagger_documentation.DTOs.CustomerDTOs;
using ASP_09._Swagger_documentation.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASP_09._Swagger_documentation.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService service)
    {
        _customerService = service;
    }

    /// <summary>Получить список клиентов с пагинацией, фильтрацией и сортировкой</summary>
    [HttpGet]
    public async Task<ActionResult<CustomerPagedResponseDto>> GetAll([FromQuery] GetCustomersQueryDto query)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _customerService.GetPagedAsync(GetUserId(), query);
        return Ok(result);
    }

    /// <summary>Получить клиента по ID</summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<CustomerResponseDto>> GetById(int id)
    {
        var customer = await _customerService.GetByIdAsync(GetUserId(), id);
        if (customer is null) return NotFound($"Customer with ID {id} not found");
        return Ok(customer);
    }

    /// <summary>Создать клиента</summary>
    [HttpPost]
    public async Task<ActionResult<CustomerResponseDto>> Create([FromBody] CreateCustomerDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _customerService.CreateAsync(GetUserId(), dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>Обновить клиента</summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<CustomerResponseDto>> Update(int id, [FromBody] UpdateCustomerDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _customerService.UpdateAsync(GetUserId(), id, dto);
        if (result is null) return NotFound($"Customer with ID {id} not found");
        return Ok(result);
    }

    /// <summary>Удалить клиента (hard delete)</summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _customerService.DeleteAsync(GetUserId(), id);
        if (!result) return NotFound($"Customer with ID {id} not found or has sent invoices");
        return NoContent();
    }

    /// <summary>Архивировать клиента (soft delete)</summary>
    [HttpPatch("{id}/archive")]
    public async Task<IActionResult> Archive(int id)
    {
        var result = await _customerService.ArchiveAsync(GetUserId(), id);
        if (!result) return NotFound($"Customer with ID {id} not found or already archived");
        return NoContent();
    }

    private int GetUserId() =>
        int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
}