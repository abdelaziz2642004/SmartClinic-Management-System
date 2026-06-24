using Clinic.Models;

namespace Clinic.States
{
    public class PendingState : IAppointmentState
    {
        public void Confirm(Appointment appointment)
        {
            appointment.Status = AppointmentStatus.Confirmed;
        }

        public void Cancel(Appointment appointment)
        {
            appointment.Status = AppointmentStatus.Cancelled;
        }

        public void Complete(Appointment appointment)
        {
            throw new InvalidOperationException(
                "لا يمكن إنهاء موعد لم يتم تأكيده بعد");
        }

        public void UndoCancellation(Appointment appointment)
        {
            throw new InvalidOperationException(
                "الموعد ليس ملغياً — لا يمكن التراجع عن الإلغاء");
        }
    }
}