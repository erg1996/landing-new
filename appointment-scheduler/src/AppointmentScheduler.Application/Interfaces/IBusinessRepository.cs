using AppointmentScheduler.Domain.Entities;

namespace AppointmentScheduler.Application.Interfaces;

public interface IBusinessRepository
{
    Task<Business?> GetByIdAsync(Guid id);
    Task AddAsync(Business business);
    Task SaveChangesAsync();
}
