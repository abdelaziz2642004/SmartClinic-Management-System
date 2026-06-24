namespace Clinic.DTOS
{
    /// <summary>
    /// Request DTO for adding a tag to an appointment (Decorator Pattern).
    /// </summary>
    public class AddTagDto
    {
        public string TagName { get; set; } = string.Empty;
    }

    /// <summary>
    /// Response DTO for an appointment tag.
    /// </summary>
    public class TagResponseDto
    {
        public int Id { get; set; }
        public string TagName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// Response DTO for the undo cancellation endpoint (Command Pattern).
    /// </summary>
    public class UndoCancelResponseDto
    {
        public string Message { get; set; } = string.Empty;
        public int RestoredAppointmentId { get; set; }
        public string PreviousState { get; set; } = string.Empty;
    }
}
