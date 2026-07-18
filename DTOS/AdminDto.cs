namespace Clinic.DTOS
{
    public class UserAdminDto
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string> Roles { get; set; } = new();
        public int? DoctorId { get; set; }
        public string? DoctorImagePath { get; set; }
    }

    public class ChangeRoleDto
    {
        // "Patient" or "Admin" - promoting to "Doctor" goes through PromoteDoctorDto instead,
        // since that also needs to create the Doctor profile row.
        public string Role { get; set; }
    }

    public class PromoteDoctorDto
    {
        public int SpecialtyID { get; set; }
        public decimal ConsultationFees { get; set; }
        public string? Description { get; set; }
    }
}
