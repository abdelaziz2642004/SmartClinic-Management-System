namespace Clinic.Models
{
    /// <summary>
    /// Decorator Pattern: Represents a dynamic tag attached to an appointment.
    /// Tags like VIP, Urgent, LateFee can be added without modifying the base Appointment class.
    /// </summary>
    public class AppointmentTag
    {
        public int Id { get; set; }
        public int AppointmentId { get; set; }
        public string TagName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        // Navigation property
        public Appointment Appointment { get; set; } = null!;
    }
}
