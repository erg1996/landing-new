using AppointmentScheduler.API.Extensions;
using AppointmentScheduler.Application.DTOs;
using AppointmentScheduler.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentScheduler.API.Controllers;

[ApiController]
[Route("api/business")]
public class BusinessController : ControllerBase
{
    private readonly BusinessService _service;

    public BusinessController(BusinessService service) => _service = service;

    // Public: used by public booking page
    [HttpGet("slug/{slug}")]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        var result = await _service.GetBySlugAsync(slug);
        return Ok(result);
    }

    // --- Authenticated endpoints below ---

    [Authorize]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var userId = User.GetUserId();
        await _service.ValidateOwnershipAsync(userId, id);
        var result = await _service.GetByIdAsync(id);
        return Ok(result);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetMyBusinesses()
    {
        var userId = User.GetUserId();
        var results = await _service.GetAllByUserIdAsync(userId);
        return Ok(results);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBusinessRequest request)
    {
        var userId = User.GetUserId();
        var result = await _service.CreateForUserAsync(userId, request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBusinessRequest request)
    {
        var userId = User.GetUserId();
        var result = await _service.UpdateAsync(id, userId, request);
        return Ok(result);
    }
}
