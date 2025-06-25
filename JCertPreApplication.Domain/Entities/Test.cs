namespace JCertPreApplication.Domain.Entities
{
    public class Test
    {
        public Guid testId { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string testType { get; set; }
        public int durationMinutes { get; set; }
        public Guid lessonId { get; set; }
        public Guid createdByUserId { get; set; }

        public virtual Lesson Lesson { get; set; }
        public virtual User CreatedByUser { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
        public virtual ICollection<TestAttempt> TestAttempts { get; set; }
    }
}
