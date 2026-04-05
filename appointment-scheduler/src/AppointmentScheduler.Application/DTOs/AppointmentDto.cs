namespace AppointmentScheduler.Application.DTOs;

public record CreateAppointmentRequest(
    Guid BusinessId,
    Guid ServiceId,
    string CustomerName,
    DateTime AppointmentDate);

public record AppointmentResponse(
    Guid Id,
    Guid BusinessId,
    Guid ServiceId,
    string CustomerName,
    DateTime AppointmentDate,
    int DurationMinutes,
    DateTime EndTime,
    DateTime CreatedAt);
