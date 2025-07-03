namespace JCertPreApplication.Application.Dtos.Profile
{
    public class InstructorProfileDto
    {
        public Guid UserId { get; set; }
        public string Introduction { get; set; } = null!;
        public string? Experience { get; set; }
        public string? TeachingStyle { get; set; }
        // Exclude Role and Users to avoid cycles
    }
}
