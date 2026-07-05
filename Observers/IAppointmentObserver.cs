namespace Clinic.Observers
{

    public interface IAppointmentObserver
    {
        Task OnAppointmentStatusChangedAsync(Clinic.Models.Appointment appointment, string oldStatus, string newStatus);
    }
}