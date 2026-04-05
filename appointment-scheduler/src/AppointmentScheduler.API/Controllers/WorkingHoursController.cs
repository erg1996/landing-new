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

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateWorkingHoursRequest request)
    {
        var result = await _service.CreateAsync(request);
        return Created($"/api/working-hours/{result.Id}", result);
    }
}
