using AppointmentScheduler.Application.DTOs;
using AppointmentScheduler.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentScheduler.API.Controllers;

[ApiController]
[Route("api/business")]
public class BusinessController : ControllerBase
{
    private readonly BusinessService _service;

    public BusinessController(BusinessService service) => _service = service;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBusinessRequest request)
    {
        var result = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        return Ok(result);
    }
}
