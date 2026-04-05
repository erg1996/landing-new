using AppointmentScheduler.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppointmentScheduler.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Business> Businesses => Set<Business>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<WorkingHours> WorkingHours => Set<WorkingHours>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Business>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.HasOne(e => e.Business)
                .WithMany(b => b.Services)
                .HasForeignKey(e => e.BusinessId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CustomerName).IsRequired().HasMaxLength(200);
            entity.Ignore(e => e.EndTime); // Computed property, not stored
            entity.HasIndex(e => new { e.BusinessId, e.AppointmentDate });
            entity.HasOne(e => e.Business)
                .WithMany(b => b.Appointments)
                .HasForeignKey(e => e.BusinessId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Service)
                .WithMany()
                .HasForeignKey(e => e.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<WorkingHours>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.BusinessId, e.DayOfWeek }).IsUnique();
            entity.HasOne(e => e.Business)
                .WithMany(b => b.WorkingHours)
                .HasForeignKey(e => e.BusinessId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
