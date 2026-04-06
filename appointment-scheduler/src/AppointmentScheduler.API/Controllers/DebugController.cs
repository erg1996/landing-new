using AppointmentScheduler.API.Extensions;
using AppointmentScheduler.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppointmentScheduler.API.Controllers;

/// <summary>
/// Temporary debug controller — remove before production.
/// </summary>
[ApiController]
[Route("api/debug")]
public class DebugController : ControllerBase
{
    private readonly AppDbContext _context;

    public DebugController(AppDbContext context) => _context = context;

    [Authorize]
    [HttpGet("whoami")]
    public async Task<IActionResult> WhoAmI()
    {
        var userId = User.GetUserId();
        var user = await _context.Users.FindAsync(userId);

        var allBusinesses = await _context.Businesses.Select(b => new { b.Id, b.Name, b.Slug }).ToListAsync();
        var allUserBusinesses = await _context.UserBusinesses.ToListAsync();
        var myUserBusinesses = allUserBusinesses.Where(ub => ub.UserId == userId).ToList();

        var myBusinessIds = myUserBusinesses.Select(ub => ub.BusinessId).ToHashSet();
        var myBusinesses = allBusinesses.Where(b => myBusinessIds.Contains(b.Id)).ToList();

        return Ok(new
        {
            authenticatedUserId = userId,
            userEmail = user?.Email,
            userName = user?.FullName,
            userLegacyBusinessId = user?.BusinessId,
            totalBusinessesInDb = allBusinesses.Count,
            totalUserBusinessRecords = allUserBusinesses.Count,
            myUserBusinessRecords = myUserBusinesses.Select(ub => new { ub.UserId, ub.BusinessId }),
            myBusinesses,
            allBusinesses,
            allUserBusinessLinks = allUserBusinesses.Select(ub => new { ub.UserId, ub.BusinessId })
        });
    }

    [HttpGet("tables")]
    public IActionResult CheckTables()
    {
        var tables = new List<string>();
        using var connection = _context.Database.GetDbConnection();
        connection.Open();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' ORDER BY name;";
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            tables.Add(reader.GetString(0));

        return Ok(new { tables, hasUserBusinessesTable = tables.Contains("UserBusinesses") });
    }
}
