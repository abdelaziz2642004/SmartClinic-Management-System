namespace Clinic.Models
{
    public class Appointment
    {
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public AppointmentStatus Status { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }

        public User Patient { get; set; }
        public Doctor Doctor { get; set; }
        public Report Report { get; set; }
        public Payment Payment { get; set; }
    }
}
