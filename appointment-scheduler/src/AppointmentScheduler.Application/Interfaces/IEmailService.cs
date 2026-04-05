namespace AppointmentScheduler.Application.Interfaces;

public interface IEmailService
{
    Task SendAppointmentConfirmationAsync(
        string toEmail,
        string customerName,
        string businessName,
        string serviceName,
        DateTime appointmentDate,
        int durationMinutes);
}
