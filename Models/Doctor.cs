namespace Clinic.Models
{
    public class Doctor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public decimal ConsultationFees { get; set; }
        public string Description { get; set; }
        public int SpecialtyID { get; set; }

        public Specialty Specialty { get; set; }
        public ICollection<Appointment> Appointments { get; set; }
    }
}
