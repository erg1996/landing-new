namespace AppointmentScheduler.Application.DTOs;

public record CreateBusinessRequest(string Name);

public record BusinessResponse(Guid Id, string Name, string Slug, DateTime CreatedAt);
