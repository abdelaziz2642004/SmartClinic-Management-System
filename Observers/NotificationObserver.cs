namespace Clinic.Observers
{
    public class NotificationObserver : IAppointmentObserver
    {
        public async Task OnAppointmentStatusChangedAsync(Clinic.Models.Appointment appointment, string oldStatus, string newStatus)
        {
            // Mock Email
            await SendEmailAsync(appointment, oldStatus, newStatus);

            // Mock SMS
            await SendSmsAsync(appointment, oldStatus, newStatus);
        }

        private Task SendEmailAsync(Clinic.Models.Appointment appointment, string oldStatus, string newStatus)
        {
            Console.WriteLine($"[EMAIL MOCK] Appointment #{appointment.AppointmentId} status changed from '{oldStatus}' to '{newStatus}'. Email sent to patient.");
            return Task.CompletedTask;
        }

        private Task SendSmsAsync(Clinic.Models.Appointment appointment, string oldStatus, string newStatus)
        {
            Console.WriteLine($"[SMS MOCK] Appointment #{appointment.AppointmentId} status changed from '{oldStatus}' to '{newStatus}'. SMS sent to patient.");
            return Task.CompletedTask;
        }
    }
}