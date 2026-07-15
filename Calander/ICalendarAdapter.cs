namespace Clinic.Calendar
{
    
    public interface ICalendarAdapter
    {
        Task<string> CreateEventAsync(Clinic.Models.Appointment appointment);
    }
}