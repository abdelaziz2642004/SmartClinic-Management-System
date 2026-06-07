using Clinic.Models;

namespace Clinic.States
{
    public interface IAppointmentState
    {
        void Confirm(Appointment appointment);
        void Cancel(Appointment appointment);
        void Complete(Appointment appointment);
    }
}