namespace AppointmentScheduler.Application.DTOs;

public record RegisterRequest(string Email, string Password, string FullName, string BusinessName);

public record LoginRequest(string Email, string Password);

public record AuthResponse(string Token, Guid UserId, string Email, string FullName, Guid BusinessId, string BusinessName, string BusinessSlug);
