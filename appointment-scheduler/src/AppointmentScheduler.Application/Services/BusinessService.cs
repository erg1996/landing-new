using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
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
        var slug = GenerateSlug(request.Name);

        // Ensure slug uniqueness by appending a short suffix if needed
        var existing = await _repository.GetBySlugAsync(slug);
        if (existing != null)
        {
            slug = $"{slug}-{Guid.NewGuid().ToString()[..4]}";
        }

        var business = new Business
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Slug = slug,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(business);
        await _repository.SaveChangesAsync();

        return ToResponse(business);
    }

    public async Task<BusinessResponse> GetByIdAsync(Guid id)
    {
        var business = await _repository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Business with id '{id}' not found.");

        return ToResponse(business);
    }

    public async Task<BusinessResponse> GetBySlugAsync(string slug)
    {
        var business = await _repository.GetBySlugAsync(slug)
            ?? throw new NotFoundException($"Business with slug '{slug}' not found.");

        return ToResponse(business);
    }

    public async Task<List<BusinessResponse>> GetAllAsync()
    {
        var businesses = await _repository.GetAllAsync();
        return businesses.Select(ToResponse).ToList();
    }

    private static BusinessResponse ToResponse(Business b) =>
        new(b.Id, b.Name, b.Slug, b.CreatedAt);

    /// <summary>
    /// Generates a URL-safe slug from a business name.
    /// "Mi Barbería Cool" → "mi-barberia-cool"
    /// </summary>
    public static string GenerateSlug(string name)
    {
        // Normalize and remove diacritics (á→a, ñ→n, etc.)
        var normalized = name.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();
        foreach (var c in normalized)
        {
            var category = CharUnicodeInfo.GetUnicodeCategory(c);
            if (category != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }

        var slug = sb.ToString().Normalize(NormalizationForm.FormC).ToLowerInvariant();

        // Replace non-alphanumeric with hyphens
        slug = Regex.Replace(slug, @"[^a-z0-9]+", "-");

        // Trim leading/trailing hyphens
        slug = slug.Trim('-');

        return slug;
    }
}
