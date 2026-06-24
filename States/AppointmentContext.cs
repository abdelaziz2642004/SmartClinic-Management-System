using Clinic.Models;

namespace Clinic.States
{
    public class AppointmentContext
    {
        private readonly Appointment _appointment;
        private IAppointmentState _state;

        public AppointmentContext(Appointment appointment)
        {
            _appointment = appointment;
            _state = GetState(appointment.Status);
        }

        private IAppointmentState GetState(AppointmentStatus status)
        {
            return status switch
            {
                AppointmentStatus.Pending => new PendingState(),
                AppointmentStatus.Confirmed => new ConfirmedState(),
                AppointmentStatus.Cancelled => new CancelledState(),
                AppointmentStatus.Completed => new CompletedState(),
                _ => throw new InvalidOperationException("حالة غير معروفة")
            };
        }

        public void Confirm() => _state.Confirm(_appointment);
        public void Cancel() => _state.Cancel(_appointment);
        public void Complete() => _state.Complete(_appointment);

        /// <summary>
        /// Command Pattern: Undo the last cancellation and restore to Pending.
        /// </summary>
        public void UndoCancellation() => _state.UndoCancellation(_appointment);
    }
}