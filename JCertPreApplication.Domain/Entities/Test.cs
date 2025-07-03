namespace JCertPreApplication.Domain.Entities
{
    public class Test
    {
        public Guid testId { get; set; }
        public string title { get; set; } = null!;
        public string description { get; set; } = null!;
        public string testType { get; set; } = null!;
        public int durationMinutes { get; set; }
        public Guid? lessonId { get; set; }
        public Guid createdByUserId { get; set; }

        public virtual Lesson Lesson { get; set; } = null!;
        public virtual User CreatedByUser { get; set; } = null!;
        public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
        public virtual ICollection<TestAttempt> TestAttempts { get; set; } = new List<TestAttempt>();
        public virtual ICollection<StudyPlanItem> StudyPlanItems { get; set; } = new List<StudyPlanItem>();
    }
}
