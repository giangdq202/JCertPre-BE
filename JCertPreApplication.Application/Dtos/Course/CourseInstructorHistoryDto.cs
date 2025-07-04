namespace JCertPreApplication.Application.Dtos.Course
{
    public class CourseInstructorHistoryDto
    {
        public Guid InstructorId { get; set; }
        public string InstructorName { get; set; } = string.Empty;
        public DateTime AssignedOn { get; set; }
        public DateTime? LeftOn { get; set; }
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
    }
} 