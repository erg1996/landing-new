using AppointmentScheduler.Application.DTOs;
using AppointmentScheduler.Application.Exceptions;
using AppointmentScheduler.Application.Interfaces;
using AppointmentScheduler.Domain.Entities;

namespace AppointmentScheduler.Application.Services;

public class BusinessService
{
    private readonly IBusinessRepository _repository;

    public BusinessService(IBusinessRepository repository)
    {
        _repository = repository;
    }

    public async Task<BusinessResponse> CreateAsync(CreateBusinessRequest request)
    {
        var business = new Business
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(business);
        await _repository.SaveChangesAsync();

        return new BusinessResponse(business.Id, business.Name, business.CreatedAt);
    }

    public async Task<BusinessResponse> GetByIdAsync(Guid id)
    {
        var business = await _repository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Business with id '{id}' not found.");

        return new BusinessResponse(business.Id, business.Name, business.CreatedAt);
    }
}
