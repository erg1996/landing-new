namespace AppointmentScheduler.Application.DTOs;

public record CreateBusinessRequest(string Name);

public record UpdateBusinessRequest(string? Name, string? BrandColor, string? LogoUrl);

public record BusinessResponse(Guid Id, string Name, string Slug, string? LogoUrl, string? BrandColor, DateTime CreatedAt);
