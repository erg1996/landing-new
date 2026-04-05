using AppointmentScheduler.Application.DTOs;
using AppointmentScheduler.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentScheduler.API.Controllers;

[ApiController]
[Route("api/blocked-dates")]
public class BlockedDatesController : ControllerBase
{
    private readonly BlockedDateService _service;

    public BlockedDatesController(BlockedDateService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetByBusinessId([FromQuery] Guid businessId)
    {
        var results = await _service.GetByBusinessIdAsync(businessId);
        return Ok(results);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBlockedDateRequest request)
    {
        var result = await _service.CreateAsync(request);
        return Created($"/api/blocked-dates/{result.Id}", result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, [FromQuery] Guid businessId)
    {
        await _service.DeleteAsync(id, businessId);
        return NoContent();
    }
}
