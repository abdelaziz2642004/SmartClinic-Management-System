namespace Clinic.DTOS
{
    public class CreateAppointmentDto
    {
        // PatientId is intentionally NOT here anymore - the server derives it
        // from the authenticated user's JWT token (see AppointmentsController.Create).
        public int DoctorId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public string? Message { get; set; }
    }

    public class AppointmentResponseDto
    {
        public int AppointmentId { get; set; }
        public string PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Message { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class PatientInfoDto
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
    }

    public class DoctorInfoDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? SpecialtyName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public decimal ConsultationFees { get; set; }
    }

    /// <summary>
    /// Safe appointment representation for API responses. The raw Appointment entity
    /// carries the full Patient (IdentityUser, including PasswordHash) and Doctor
    /// (including the plaintext Password field) navigation properties, which must never
    /// be serialized straight back to the client.
    /// </summary>
    public class AppointmentDetailDto
    {
        public int AppointmentId { get; set; }
        public string PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public PatientInfoDto? Patient { get; set; }
        public DoctorInfoDto? Doctor { get; set; }
        public List<TagResponseDto> Tags { get; set; } = new();
    }
}