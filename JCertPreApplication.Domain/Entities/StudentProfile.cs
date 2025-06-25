namespace JCertPreApplication.Domain.Entities
{
    public class StudentProfile
    {
        public Guid userId { get; set; }
        public string currentLevel { get; set; }
        public string learningGoals { get; set; }

        // Navigation property
        public virtual User User { get; set; }
    }
}
