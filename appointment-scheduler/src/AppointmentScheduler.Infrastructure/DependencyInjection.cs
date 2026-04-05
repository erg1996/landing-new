using AppointmentScheduler.Application.Interfaces;
using AppointmentScheduler.Application.Services;
using AppointmentScheduler.Infrastructure.Data;
using AppointmentScheduler.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AppointmentScheduler.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

        // Repositories
        services.AddScoped<IBusinessRepository, BusinessRepository>();
        services.AddScoped<IServiceRepository, ServiceRepository>();
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        services.AddScoped<IWorkingHoursRepository, WorkingHoursRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        // Application services
        services.AddScoped<BusinessService>();
        services.AddScoped<ServiceService>();
        services.AddScoped<WorkingHoursService>();
        services.AddScoped<AvailabilityService>();
        services.AddScoped<AppointmentService>();
        services.AddScoped<AnalyticsService>();
        services.AddScoped<AuthService>(sp =>
        {
            var userRepo = sp.GetRequiredService<IUserRepository>();
            var bizRepo = sp.GetRequiredService<IBusinessRepository>();
            var jwtSecret = configuration["Jwt:Secret"] ?? "SchedulePro-Default-Secret-Key-Change-In-Production-Min32Chars!";
            return new AuthService(userRepo, bizRepo, jwtSecret);
        });

        return services;
    }
}
