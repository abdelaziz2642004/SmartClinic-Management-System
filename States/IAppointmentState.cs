using Clinic.Models;

namespace Clinic.States
{
    public interface IAppointmentState
    {
        void Confirm(Appointment appointment);
        void Cancel(Appointment appointment);
        void Complete(Appointment appointment);

        /// <summary>
        /// Command Pattern: Undo a cancellation and restore to Pending state.
        /// Only valid when the appointment is in Cancelled state.
        /// </summary>
        void UndoCancellation(Appointment appointment);
    }
}