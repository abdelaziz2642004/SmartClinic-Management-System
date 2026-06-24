namespace Clinic.Models
{
    /// <summary>
    /// Command Pattern: Stores cancellation history so it can be undone.
    /// Each record captures the appointment's previous state before cancellation.
    /// </summary>
    public class CancellationCommand
    {
        public int Id { get; set; }
        public int AppointmentId { get; set; }
        public string PreviousState { get; set; } = string.Empty;
        public DateTime CancelledAt { get; set; }
        public string? CancelledByUserId { get; set; }
        public bool IsUndone { get; set; } = false;

        // Navigation properties
        public Appointment Appointment { get; set; } = null!;
        public User? CancelledByUser { get; set; }
    }
}
