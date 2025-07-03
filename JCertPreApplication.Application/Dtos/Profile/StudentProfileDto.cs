namespace JCertPreApplication.Application.Dtos.Profile
{
    public class StudentProfileDto
    {
        public Guid UserId { get; set; }
        public string CurrentLevel { get; set; } = null!;
        public string LearningGoals { get; set; } = null!;
    }
}
