using AppointmentScheduler.Application.DTOs;
using AppointmentScheduler.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentScheduler.API.Controllers;

[ApiController]
[Route("api/appointments")]
public class AppointmentsController : ControllerBase
{
    private readonly AppointmentService _service;

    public AppointmentsController(AppointmentService service) => _service = service;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAppointmentRequest request)
    {
        var result = await _service.CreateAsync(request);
        return Created($"/api/appointments/{result.Id}", result);
    }

    [HttpGet]
    public async Task<IActionResult> GetByBusiness([FromQuery] Guid businessId)
    {
        var results = await _service.GetByBusinessIdAsync(businessId);
        return Ok(results);
    }
}
