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
        public string? ImagePath { get; set; }

        // Optional link to the AspNetUsers account that owns this doctor profile
        // (null for the demo/seeded doctors that were never promoted through the admin panel).
        public string? UserId { get; set; }
        public User? User { get; set; }

        public Specialty Specialty { get; set; }
        public ICollection<Appointment> Appointments { get; set; }

    }
}
