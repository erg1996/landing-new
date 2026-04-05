using AppointmentScheduler.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentScheduler.API.Controllers;

[ApiController]
[Route("api/analytics")]
public class AnalyticsController : ControllerBase
{
    private readonly AnalyticsService _service;

    public AnalyticsController(AnalyticsService service) => _service = service;

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard([FromQuery] Guid businessId)
    {
        var result = await _service.GetDashboardAsync(businessId);
        return Ok(result);
    }
}
