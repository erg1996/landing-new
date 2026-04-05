using AppointmentScheduler.Application.DTOs;
using AppointmentScheduler.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentScheduler.API.Controllers;

[ApiController]
[Route("api/working-hours")]
public class WorkingHoursController : ControllerBase
{
    private readonly WorkingHoursService _service;

    public WorkingHoursController(WorkingHoursService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetByBusinessId([FromQuery] Guid businessId)
    {
        var results = await _service.GetByBusinessIdAsync(businessId);
        return Ok(results);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateWorkingHoursRequest request)
    {
        var result = await _service.CreateAsync(request);
        return Created($"/api/working-hours/{result.Id}", result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateWorkingHoursRequest request)
    {
        var result = await _service.UpdateAsync(id, request);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
