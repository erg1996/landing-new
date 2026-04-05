using AppointmentScheduler.Domain.Entities;

namespace AppointmentScheduler.Infrastructure.Data;

public static class SeedData
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (context.Businesses.Any())
            return;

        var businessId = Guid.NewGuid();
        var business = new Business
        {
            Id = businessId,
            Name = "Demo Barbershop",
            CreatedAt = DateTime.UtcNow
        };

        var haircut = new Service
        {
            Id = Guid.NewGuid(),
            BusinessId = businessId,
            Name = "Haircut",
            DurationMinutes = 30
        };

        var beardTrim = new Service
        {
            Id = Guid.NewGuid(),
            BusinessId = businessId,
            Name = "Beard Trim",
            DurationMinutes = 15
        };

        var fullService = new Service
        {
            Id = Guid.NewGuid(),
            BusinessId = businessId,
            Name = "Haircut + Beard",
            DurationMinutes = 45
        };

        // Monday to Friday, 9:00 - 17:00
        var workingHours = new List<WorkingHours>();
        for (int day = 1; day <= 5; day++)
        {
            workingHours.Add(new WorkingHours
            {
                Id = Guid.NewGuid(),
                BusinessId = businessId,
                DayOfWeek = day,
                StartTime = new TimeSpan(9, 0, 0),
                EndTime = new TimeSpan(17, 0, 0)
            });
        }

        // Saturday 10:00 - 14:00
        workingHours.Add(new WorkingHours
        {
            Id = Guid.NewGuid(),
            BusinessId = businessId,
            DayOfWeek = 6,
            StartTime = new TimeSpan(10, 0, 0),
            EndTime = new TimeSpan(14, 0, 0)
        });

        context.Businesses.Add(business);
        context.Services.AddRange(haircut, beardTrim, fullService);
        context.WorkingHours.AddRange(workingHours);
        await context.SaveChangesAsync();
    }
}
