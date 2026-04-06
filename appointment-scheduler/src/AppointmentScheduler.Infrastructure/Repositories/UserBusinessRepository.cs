using AppointmentScheduler.Application.Interfaces;
using AppointmentScheduler.Domain.Entities;
using AppointmentScheduler.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AppointmentScheduler.Infrastructure.Repositories;

public class UserBusinessRepository : IUserBusinessRepository
{
    private readonly AppDbContext _context;

    public UserBusinessRepository(AppDbContext context) => _context = context;

    public async Task<List<Business>> GetBusinessesByUserIdAsync(Guid userId) =>
        await _context.UserBusinesses
            .Where(ub => ub.UserId == userId)
            .Include(ub => ub.Business)
            .OrderByDescending(ub => ub.CreatedAt)
            .Select(ub => ub.Business)
            .ToListAsync();

    public async Task<bool> HasAccessAsync(Guid userId, Guid businessId) =>
        await _context.UserBusinesses
            .AnyAsync(ub => ub.UserId == userId && ub.BusinessId == businessId);

    public async Task AddAsync(UserBusiness userBusiness) =>
        await _context.UserBusinesses.AddAsync(userBusiness);

    public async Task SaveChangesAsync() =>
        await _context.SaveChangesAsync();
}
