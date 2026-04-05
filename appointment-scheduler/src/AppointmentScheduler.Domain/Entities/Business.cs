namespace AppointmentScheduler.Domain.Entities;

public class Business
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public List<Service> Services { get; set; } = new();
    public List<Appointment> Appointments { get; set; } = new();
    public List<WorkingHours> WorkingHours { get; set; } = new();
}
