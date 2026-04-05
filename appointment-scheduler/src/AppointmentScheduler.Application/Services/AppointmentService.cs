using AppointmentScheduler.Application.DTOs;
using AppointmentScheduler.Application.Exceptions;
using AppointmentScheduler.Application.Interfaces;
using AppointmentScheduler.Domain.Entities;

namespace AppointmentScheduler.Application.Services;

public class AppointmentService
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IServiceRepository _serviceRepository;
    private readonly IBusinessRepository _businessRepository;
    private readonly IWorkingHoursRepository _workingHoursRepository;
    private readonly IEmailService _emailService;

    public AppointmentService(
        IAppointmentRepository appointmentRepository,
        IServiceRepository serviceRepository,
        IBusinessRepository businessRepository,
        IWorkingHoursRepository workingHoursRepository,
        IEmailService emailService)
    {
        _appointmentRepository = appointmentRepository;
        _serviceRepository = serviceRepository;
        _businessRepository = businessRepository;
        _workingHoursRepository = workingHoursRepository;
        _emailService = emailService;
    }

    public async Task<AppointmentResponse> CreateAsync(CreateAppointmentRequest request)
    {
        var business = await _businessRepository.GetByIdAsync(request.BusinessId)
            ?? throw new NotFoundException($"Business with id '{request.BusinessId}' not found.");

        var service = await _serviceRepository.GetByIdAsync(request.ServiceId)
            ?? throw new NotFoundException($"Service with id '{request.ServiceId}' not found.");

        var appointmentDate = request.AppointmentDate;
        var endTime = appointmentDate.AddMinutes(service.DurationMinutes);

        // Validate within working hours
        int dayOfWeek = (int)appointmentDate.DayOfWeek;
        var workingHoursList = await _workingHoursRepository.GetByBusinessIdAndDayAsync(request.BusinessId, dayOfWeek);

        if (workingHoursList.Count == 0)
            throw new ConflictException("The business is closed on the requested day.");

        var wh = workingHoursList[0];
        var dayStart = appointmentDate.Date + wh.StartTime;
        var dayEnd = appointmentDate.Date + wh.EndTime;

        if (appointmentDate < dayStart || endTime > dayEnd)
            throw new ConflictException("The appointment falls outside of working hours.");

        // Validate no overlap with existing appointments
        var existingAppointments = await _appointmentRepository.GetByBusinessIdAndDateAsync(request.BusinessId, appointmentDate);

        bool hasConflict = existingAppointments.Any(a =>
            appointmentDate < a.EndTime && a.AppointmentDate < endTime);

        if (hasConflict)
            throw new ConflictException("The requested time slot conflicts with an existing appointment.");

        var appointment = new Appointment
        {
            Id = Guid.NewGuid(),
            BusinessId = request.BusinessId,
            ServiceId = request.ServiceId,
            CustomerName = request.CustomerName,
            CustomerEmail = request.CustomerEmail?.Trim(),
            AppointmentDate = appointmentDate,
            DurationMinutes = service.DurationMinutes,
            CreatedAt = DateTime.UtcNow
        };

        await _appointmentRepository.AddAsync(appointment);
        await _appointmentRepository.SaveChangesAsync();

        // Send confirmation email (fire-and-forget, won't block response)
        if (!string.IsNullOrWhiteSpace(appointment.CustomerEmail))
        {
            _ = _emailService.SendAppointmentConfirmationAsync(
                appointment.CustomerEmail,
                appointment.CustomerName,
                business.Name,
                service.Name,
                appointment.AppointmentDate,
                appointment.DurationMinutes);
        }

        return ToResponse(appointment);
    }

    public async Task<List<AppointmentResponse>> GetByBusinessIdAsync(Guid businessId)
    {
        var appointments = await _appointmentRepository.GetByBusinessIdAsync(businessId);
        return appointments.Select(ToResponse).ToList();
    }

    private static AppointmentResponse ToResponse(Appointment a) =>
        new(a.Id, a.BusinessId, a.ServiceId, a.CustomerName, a.CustomerEmail,
            a.AppointmentDate, a.DurationMinutes, a.EndTime, a.CreatedAt);
}
