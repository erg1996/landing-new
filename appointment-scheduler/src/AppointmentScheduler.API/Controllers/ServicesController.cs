using AppointmentScheduler.Application.DTOs;
using AppointmentScheduler.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentScheduler.API.Controllers;

[ApiController]
[Route("api/services")]
public class ServicesController : ControllerBase
{
    private readonly ServiceService _service;

    public ServicesController(ServiceService service) => _service = service;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateServiceRequest request)
    {
        var result = await _service.CreateAsync(request);
        return Created($"/api/services/{result.Id}", result);
    }

    [HttpGet]
    public async Task<IActionResult> GetByBusiness([FromQuery] Guid businessId)
    {
        var results = await _service.GetByBusinessIdAsync(businessId);
        return Ok(results);
    }
}
