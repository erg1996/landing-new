using AppointmentScheduler.Application.DTOs;
using AppointmentScheduler.Application.Interfaces;

namespace AppointmentScheduler.Application.Services;

public class AnalyticsService
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IServiceRepository _serviceRepository;

    public AnalyticsService(IAppointmentRepository appointmentRepository, IServiceRepository serviceRepository)
    {
        _appointmentRepository = appointmentRepository;
        _serviceRepository = serviceRepository;
    }

    public async Task<DashboardAnalytics> GetDashboardAsync(Guid businessId)
    {
        var appointments = await _appointmentRepository.GetByBusinessIdAsync(businessId);
        var services = await _serviceRepository.GetByBusinessIdAsync(businessId);
        var today = DateTime.UtcNow.Date;
        var todayAppointments = appointments.Count(a => a.AppointmentDate.Date == today);

        // Top service by appointment count
        ServiceStat? topService = null;
        if (appointments.Count > 0)
        {
            var grouped = appointments
                .GroupBy(a => a.ServiceId)
                .OrderByDescending(g => g.Count())
                .First();
            var svc = services.FirstOrDefault(s => s.Id == grouped.Key);
            topService = new ServiceStat(svc?.Name ?? "Desconocido", grouped.Count());
        }

        // Busiest and quietest hours
        HourStat? busiestHour = null;
        HourStat? quietestHour = null;
        if (appointments.Count > 0)
        {
            var hourGroups = appointments
                .GroupBy(a => a.AppointmentDate.Hour)
                .Select(g => new HourStat(g.Key, g.Count()))
                .OrderByDescending(h => h.Count)
                .ToList();

            busiestHour = hourGroups.First();
            quietestHour = hourGroups.Last();
        }

        return new DashboardAnalytics(
            appointments.Count,
            services.Count,
            todayAppointments,
            topService,
            busiestHour,
            quietestHour);
    }
}
