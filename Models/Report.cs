namespace Clinic.Models
{
    public class Report
    {
        public int ReportId { get; set; }
        public int AppointmentId { get; set; }
        public string Diagnosis { get; set; }
        public string Message { get; set; }
        public DateTime ReportDate { get; set; }

        public Appointment Appointment { get; set; }

    }
}
