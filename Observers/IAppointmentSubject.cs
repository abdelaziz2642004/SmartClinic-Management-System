

namespace Clinic.Observers
{
     public interface IAppointmentSubject
    {
        void Subscribe(IAppointmentObserver observer);
        void Unsubscribe(IAppointmentObserver observer);
        Task NotifyObserversAsync(Clinic.Models.Appointment appointment, string oldStatus, string newStatus);
    }
}