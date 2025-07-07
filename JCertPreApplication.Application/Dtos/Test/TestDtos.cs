namespace JCertPreApplication.Application.Dtos.Test
{
    public class TestDto
    {
        public Guid TestId { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string TestType { get; set; } = null!;
        public int DurationMinutes { get; set; }
        public Guid? LessonId { get; set; }
        public Guid CreatedByUserId { get; set; }
    }
}