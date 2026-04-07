using AppointmentScheduler.Application.DTOs;
using AppointmentScheduler.Application.Interfaces;
using AppointmentScheduler.Domain.Entities;

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
        var allAppointments = await _appointmentRepository.GetByBusinessIdAsync(businessId);
        var services = await _serviceRepository.GetByBusinessIdAsync(businessId);
        var today = DateTime.UtcNow.Date;

        var cancelled = allAppointments.Count(a => a.Status == AppointmentStatus.Cancelled);
        // Exclude cancelled from all stats
        var active = allAppointments.Where(a => a.Status != AppointmentStatus.Cancelled).ToList();

        var todayAppointments = active.Count(a => a.AppointmentDate.Date == today);

        // Top service by appointment count (active only)
        ServiceStat? topService = null;
        if (active.Count > 0)
        {
            var grouped = active
                .GroupBy(a => a.ServiceId)
                .OrderByDescending(g => g.Count())
                .First();
            var svc = services.FirstOrDefault(s => s.Id == grouped.Key);
            topService = new ServiceStat(svc?.Name ?? "Desconocido", grouped.Count());
        }

        // Busiest and quietest hours (active only)
        HourStat? busiestHour = null;
        HourStat? quietestHour = null;
        if (active.Count > 0)
        {
            var hourGroups = active
                .GroupBy(a => a.AppointmentDate.Hour)
                .Select(g => new HourStat(g.Key, g.Count()))
                .OrderByDescending(h => h.Count)
                .ToList();

            busiestHour = hourGroups.First();
            quietestHour = hourGroups.Last();
        }

        return new DashboardAnalytics(
            active.Count,
            services.Count,
            todayAppointments,
            cancelled,
            topService,
            busiestHour,
            quietestHour);
    }
}
