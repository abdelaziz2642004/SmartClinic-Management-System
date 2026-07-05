namespace Clinic.Observers
{
    public class AppointmentNotifier : IAppointmentSubject
    {
        private readonly List<IAppointmentObserver> _observers = new();

    
        public AppointmentNotifier(IEnumerable<IAppointmentObserver> observers)
        {
            _observers.AddRange(observers);
        }

        public void Subscribe(IAppointmentObserver observer)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }
        }

        public void Unsubscribe(IAppointmentObserver observer)
        {
            _observers.Remove(observer);
        }

        public async Task NotifyObserversAsync(Clinic.Models.Appointment appointment, string oldStatus, string newStatus)
        {
            foreach (var observer in _observers)
            {
                await observer.OnAppointmentStatusChangedAsync(appointment, oldStatus, newStatus);
            }
        }
    }
}