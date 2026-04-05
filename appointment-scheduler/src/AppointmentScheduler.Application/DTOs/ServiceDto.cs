namespace AppointmentScheduler.Application.DTOs;

public record CreateServiceRequest(Guid BusinessId, string Name, int DurationMinutes);

public record ServiceResponse(Guid Id, Guid BusinessId, string Name, int DurationMinutes);
