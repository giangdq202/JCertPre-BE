using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.Domain.Entities
{
    public class Test
    {
        public Guid testId { get; set; }
        public string title { get; set; } = null!;
        public string description { get; set; } = null!;
        public TestType testType { get; set; } // Changed to enum
        public int durationMinutes { get; set; }
        public Guid? lessonId { get; set; }
        public Guid createdByUserId { get; set; }
        public DateTime? availableFrom { get; set; } // New
        public DateTime? availableTo { get; set; }   // New
        public int maxAttempts { get; set; }        // New
        public TestStatus status { get; set; } // <-- Added

        public virtual Lesson Lesson { get; set; } = null!;
        public virtual User CreatedByUser { get; set; } = null!;
        public virtual ICollection<TestQuestion> TestQuestions { get; set; } = new List<TestQuestion>();
        public virtual ICollection<TestAttempt> TestAttempts { get; set; } = new List<TestAttempt>();
        public virtual ICollection<StudyPlanItem> StudyPlanItems { get; set; } = new List<StudyPlanItem>();
    }
}
