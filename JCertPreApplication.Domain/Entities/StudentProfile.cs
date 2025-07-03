namespace JCertPreApplication.Domain.Entities
{
    public class StudentProfile
    {
        public Guid userId { get; set; }
        public string currentLevel { get; set; } = null!;
        public string learningGoals { get; set; } = null!;

        // Navigation property
        public virtual User User { get; set; } = null!;
    }
}
