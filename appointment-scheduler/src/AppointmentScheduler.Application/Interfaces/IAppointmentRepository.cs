using AppointmentScheduler.Domain.Entities;

namespace AppointmentScheduler.Application.Interfaces;

public interface IAppointmentRepository
{
    Task<Appointment?> GetByIdAsync(Guid id);
    Task<List<Appointment>> GetByBusinessIdAsync(Guid businessId);
    Task<List<Appointment>> GetByBusinessIdAndDateAsync(Guid businessId, DateTime date);
    Task AddAsync(Appointment appointment);
    Task SaveChangesAsync();
}
