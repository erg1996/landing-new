using AppointmentScheduler.Domain.Entities;

namespace AppointmentScheduler.Application.Interfaces;

public interface IServiceRepository
{
    Task<Service?> GetByIdAsync(Guid id);
    Task<List<Service>> GetByBusinessIdAsync(Guid businessId);
    Task AddAsync(Service service);
    Task SaveChangesAsync();
}
