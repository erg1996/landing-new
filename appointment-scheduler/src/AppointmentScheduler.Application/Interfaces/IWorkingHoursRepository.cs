using AppointmentScheduler.Domain.Entities;

namespace AppointmentScheduler.Application.Interfaces;

public interface IWorkingHoursRepository
{
    Task<List<WorkingHours>> GetByBusinessIdAndDayAsync(Guid businessId, int dayOfWeek);
    Task AddAsync(WorkingHours workingHours);
    Task SaveChangesAsync();
}
