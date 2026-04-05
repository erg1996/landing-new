using AppointmentScheduler.Application.Interfaces;
using AppointmentScheduler.Domain.Entities;
using AppointmentScheduler.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AppointmentScheduler.Infrastructure.Repositories;

public class WorkingHoursRepository : IWorkingHoursRepository
{
    private readonly AppDbContext _context;

    public WorkingHoursRepository(AppDbContext context) => _context = context;

    public async Task<List<WorkingHours>> GetByBusinessIdAndDayAsync(Guid businessId, int dayOfWeek) =>
        await _context.WorkingHours
            .Where(wh => wh.BusinessId == businessId && wh.DayOfWeek == dayOfWeek)
            .ToListAsync();

    public async Task AddAsync(WorkingHours workingHours) =>
        await _context.WorkingHours.AddAsync(workingHours);

    public async Task SaveChangesAsync() =>
        await _context.SaveChangesAsync();
}
