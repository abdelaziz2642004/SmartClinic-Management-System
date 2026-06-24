using Clinic.Models;

namespace Clinic.States
{
    public class ConfirmedState : IAppointmentState
    {
        public void Confirm(Appointment appointment)
        {
            throw new InvalidOperationException(
                "الموعد مؤكد بالفعل");
        }

        public void Cancel(Appointment appointment)
        {
            appointment.Status = AppointmentStatus.Cancelled;
        }

        public void Complete(Appointment appointment)
        {
            appointment.Status = AppointmentStatus.Completed;
        }

        public void UndoCancellation(Appointment appointment)
        {
            throw new InvalidOperationException(
                "الموعد ليس ملغياً — لا يمكن التراجع عن الإلغاء");
        }
    }
}