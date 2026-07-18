namespace Clinic.DTOS
{
    public class DoctorDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public decimal ConsultationFees { get; set; }
        public string Description { get; set; }
        public int SpecialtyID { get; set; }
        public string SpecialtyName { get; set; }
        public string? ImagePath { get; set; }
    }

    public class SpecialtyDto
    {
        public int SpecialtyID { get; set; }
        public string Name { get; set; }
        public int DoctorsCount { get; set; }
    }

    public class UpdateDoctorDto
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Description { get; set; }
        public decimal ConsultationFees { get; set; }
    }
}
