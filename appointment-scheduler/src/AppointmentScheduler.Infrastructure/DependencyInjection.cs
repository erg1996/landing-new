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

        // Application services
        services.AddScoped<BusinessService>();
        services.AddScoped<ServiceService>();
        services.AddScoped<WorkingHoursService>();
        services.AddScoped<AvailabilityService>();
        services.AddScoped<AppointmentService>();

        return services;
    }
}
