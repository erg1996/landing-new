using AppointmentScheduler.Application.Interfaces;
using AppointmentScheduler.Domain.Entities;
using AppointmentScheduler.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AppointmentScheduler.Infrastructure.Repositories;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly AppDbContext _context;

    public AppointmentRepository(AppDbContext context) => _context = context;

    public async Task<Appointment?> GetByIdAsync(Guid id) =>
        await _context.Appointments.FindAsync(id);

    public async Task<List<Appointment>> GetByBusinessIdAsync(Guid businessId) =>
        await _context.Appointments
            .Where(a => a.BusinessId == businessId)
            .OrderBy(a => a.AppointmentDate)
            .ToListAsync();

    public async Task<List<Appointment>> GetByBusinessIdAndDateAsync(Guid businessId, DateTime date) =>
        await _context.Appointments
            .Where(a => a.BusinessId == businessId && a.AppointmentDate.Date == date.Date)
            .ToListAsync();

    public async Task AddAsync(Appointment appointment) =>
        await _context.Appointments.AddAsync(appointment);

    public async Task SaveChangesAsync() =>
        await _context.SaveChangesAsync();
}
