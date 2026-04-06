using AppointmentScheduler.API.Extensions;
using AppointmentScheduler.Application.DTOs;
using AppointmentScheduler.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentScheduler.API.Controllers;

[ApiController]
[Route("api/appointments")]
public class AppointmentsController : ControllerBase
{
    private readonly AppointmentService _appointmentService;
    private readonly BusinessService _businessService;

    public AppointmentsController(AppointmentService appointmentService, BusinessService businessService)
    {
        _appointmentService = appointmentService;
        _businessService = businessService;
    }

    // Public: called from public booking page (no auth required)
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAppointmentRequest request)
    {
        var result = await _appointmentService.CreateAsync(request);
        return Created($"/api/appointments/{result.Id}", result);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetByBusiness([FromQuery] Guid businessId)
    {
        var userId = User.GetUserId();
        await _businessService.ValidateOwnershipAsync(userId, businessId);
        var results = await _appointmentService.GetByBusinessIdAsync(businessId);
        return Ok(results);
    }
}
