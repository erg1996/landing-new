using AppointmentScheduler.Application.Interfaces;
using AppointmentScheduler.Domain.Entities;
using AppointmentScheduler.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AppointmentScheduler.Infrastructure.Repositories;

public class BusinessRepository : IBusinessRepository
{
    private readonly AppDbContext _context;

    public BusinessRepository(AppDbContext context) => _context = context;

    public async Task<Business?> GetByIdAsync(Guid id) =>
        await _context.Businesses.FindAsync(id);

    public async Task AddAsync(Business business) =>
        await _context.Businesses.AddAsync(business);

    public async Task SaveChangesAsync() =>
        await _context.SaveChangesAsync();
}
