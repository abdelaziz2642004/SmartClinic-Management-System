using Clinic.Models;

namespace Clinic.States
{
    public class CancelledState : IAppointmentState
    {
        public void Confirm(Appointment appointment)
        {
            throw new InvalidOperationException(
                "لا يمكن تأكيد موعد ملغي");
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
    }
}