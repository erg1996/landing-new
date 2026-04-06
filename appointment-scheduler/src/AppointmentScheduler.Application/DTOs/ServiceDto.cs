using System.ComponentModel.DataAnnotations;

namespace AppointmentScheduler.Application.DTOs;

public record CreateServiceRequest(
    [Required] Guid BusinessId,
    [Required, MaxLength(200)] string Name,
    [Range(5, 480)] int DurationMinutes);

public record ServiceResponse(Guid Id, Guid BusinessId, string Name, int DurationMinutes);
