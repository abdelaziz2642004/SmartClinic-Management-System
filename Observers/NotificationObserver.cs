namespace Clinic.Observers
{
    public class NotificationObserver : IAppointmentObserver
    {
        public async Task OnAppointmentStatusChangedAsync(Clinic.Models.Appointment appointment, string oldStatus, string newStatus)
        {
            await SendEmailAsync(appointment, oldStatus, newStatus);
            await SendSmsAsync(appointment, oldStatus, newStatus);
        }

        private Task SendEmailAsync(Clinic.Models.Appointment appointment, string oldStatus, string newStatus)
        {
            var patientName = appointment.Patient?.FirstName ?? "Patient";
            Console.WriteLine($"[EMAIL MOCK] To: {patientName} — Your appointment #{appointment.AppointmentId} status changed from '{oldStatus}' to '{newStatus}'.");
            return Task.CompletedTask;
        }

        private Task SendSmsAsync(Clinic.Models.Appointment appointment, string oldStatus, string newStatus)
        {
            var patientName = appointment.Patient?.FirstName ?? "Patient";
            Console.WriteLine($"[SMS MOCK] To: {patientName} — Your appointment #{appointment.AppointmentId} status changed from '{oldStatus}' to '{newStatus}'.");
            return Task.CompletedTask;
        }
    }
}
