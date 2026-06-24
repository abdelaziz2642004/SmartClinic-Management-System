using Clinic.Models;

namespace Clinic.States
{
    public class CancelledState : IAppointmentState
    {
        public void Confirm(Appointment appointment)
        {
            throw new InvalidOperationException(
                "لا يمكن تأكيد موعد ملغي. استخدم خاصية التراجع عن الإلغاء بدلاً من ذلك");
        }

        public void Cancel(Appointment appointment)
        {
            throw new InvalidOperationException(
                "الموعد ملغي بالفعل");
        }

        public void Complete(Appointment appointment)
        {
            throw new InvalidOperationException(
                "لا يمكن إنهاء موعد ملغي");
        }

        /// <summary>
        /// Command Pattern: Undo cancellation — transitions back to Pending.
        /// Per spec state diagram: Canceled --> Pending : Undo Command Executed
        /// </summary>
        public void UndoCancellation(Appointment appointment)
        {
            appointment.Status = AppointmentStatus.Pending;
        }
    }
}