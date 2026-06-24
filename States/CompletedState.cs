using Clinic.Models;

namespace Clinic.States
{
    public class CompletedState : IAppointmentState
    {
        public void Confirm(Appointment appointment)
        {
            throw new InvalidOperationException(
                "الموعد منتهي لا يمكن تعديله");
        }

        public void Cancel(Appointment appointment)
        {
            throw new InvalidOperationException(
                "الموعد منتهي لا يمكن إلغاؤه");
        }

        public void Complete(Appointment appointment)
        {
            throw new InvalidOperationException(
                "الموعد منتهي بالفعل");
        }

        public void UndoCancellation(Appointment appointment)
        {
            throw new InvalidOperationException(
                "الموعد ليس ملغياً — لا يمكن التراجع عن الإلغاء");
        }
    }
}