using AppointmentScheduler.Application.DTOs;
using AppointmentScheduler.Application.Exceptions;
using AppointmentScheduler.Application.Interfaces;
using AppointmentScheduler.Domain.Entities;

namespace AppointmentScheduler.Application.Services;

public class WorkingHoursService
{
    private readonly IWorkingHoursRepository _workingHoursRepository;
    private readonly IBusinessRepository _businessRepository;

    public WorkingHoursService(IWorkingHoursRepository workingHoursRepository, IBusinessRepository businessRepository)
    {
        _workingHoursRepository = workingHoursRepository;
        _businessRepository = businessRepository;
    }

    public async Task<WorkingHoursResponse> CreateAsync(CreateWorkingHoursRequest request)
    {
        _ = await _businessRepository.GetByIdAsync(request.BusinessId)
            ?? throw new NotFoundException($"Business with id '{request.BusinessId}' not found.");

        if (request.DayOfWeek < 0 || request.DayOfWeek > 6)
            throw new ConflictException("DayOfWeek must be between 0 (Sunday) and 6 (Saturday).");

        if (request.StartTime >= request.EndTime)
            throw new ConflictException("StartTime must be before EndTime.");

        var existing = await _workingHoursRepository.GetByBusinessIdAndDayAsync(request.BusinessId, request.DayOfWeek);
        if (existing.Count > 0)
            throw new ConflictException($"Working hours already defined for day {request.DayOfWeek}.");

        var workingHours = new WorkingHours
        {
            Id = Guid.NewGuid(),
            BusinessId = request.BusinessId,
            DayOfWeek = request.DayOfWeek,
            StartTime = request.StartTime,
            EndTime = request.EndTime
        };

        await _workingHoursRepository.AddAsync(workingHours);
        await _workingHoursRepository.SaveChangesAsync();

        return new WorkingHoursResponse(workingHours.Id, workingHours.BusinessId, workingHours.DayOfWeek, workingHours.StartTime, workingHours.EndTime);
    }
}
