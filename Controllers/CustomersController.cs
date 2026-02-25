namespace ASP_09._Swagger_documentation.Controllers;
using ASP_09._Swagger_documentation.DTOs.CustomerDTOs;
using ASP_09._Swagger_documentation.Services.Interfaces;
using ASP_09._Swagger_documentation.Services;
using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService service)
    {
        _customerService = service;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CustomerResponseDto>> GetById(int id)
    {
        var customer = await _customerService.GetByIdAsync(id);
        if (customer is null)
            return NotFound($"Customer with ID {id} not found");
        return Ok(customer);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerResponseDto>>> GetAll()
    {
        var projects = await _customerService.GetAllAsync();
        return Ok(projects);
    }

    [HttpPost]
    public async Task<ActionResult<CustomerResponseDto>> Create([FromBody] CreateCustomerDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _customerService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Id },
            result);
    }
    [HttpPut("{id}")]
    public async Task<ActionResult<CustomerResponseDto>> Update(int id, [FromBody] UpdateCustomerDto updateCustomerDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var updatedCustomer = await _customerService.UpdateAsync(id, updateCustomerDto);

        if (updatedCustomer is null) return NotFound($"Customer with ID {id} not found");

        return Ok(updatedCustomer);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var isDeleted = await _customerService.DeleteAsync(id);

        if (!isDeleted) return NotFound($"Customer with ID {id} not found");

        return NoContent();
    }

    [HttpPatch("{id}/archive")]
    public async Task<IActionResult> Archive(int id)
    {
        var result = await _customerService.ArchiveAsync(id);

        if (!result)
            return NotFound($"Customer with ID {id} not found or already archived");

        return NoContent();
    }

}

