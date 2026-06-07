namespace Clinic.Models
{
    public class Specialty
    {
        public int SpecialtyID { get; set; }
        public string Name { get; set; }
        public ICollection<Doctor> Doctors { get; set; }
    }
}
