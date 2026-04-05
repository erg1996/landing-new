using AppointmentScheduler.Domain.Entities;

namespace AppointmentScheduler.Application.Interfaces;

public interface IBusinessRepository
{
    Task<Business?> GetByIdAsync(Guid id);
    Task<Business?> GetBySlugAsync(string slug);
    Task<List<Business>> GetAllAsync();
    Task AddAsync(Business business);
    Task SaveChangesAsync();
}
