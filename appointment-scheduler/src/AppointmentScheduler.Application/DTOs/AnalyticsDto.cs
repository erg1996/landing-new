namespace AppointmentScheduler.Application.DTOs;

public record DashboardAnalytics(
    int TotalAppointments,
    int TotalServices,
    int TodayAppointments,
    ServiceStat? TopService,
    HourStat? BusiestHour,
    HourStat? QuietestHour);

public record ServiceStat(string Name, int Count);

public record HourStat(int Hour, int Count);
