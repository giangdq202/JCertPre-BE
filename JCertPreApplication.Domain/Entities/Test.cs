using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.Domain.Entities
{
    public class Test
    {
        public Guid testId { get; set; }
        public string title { get; set; } = null!;
        public string description { get; set; } = null!;
        public TestType testType { get; set; }
        public CourseLevel courseLevel { get; set; }
        public int durationMinutes { get; set; }
        public Guid? lessonId { get; set; }
        public Guid createdByUserId { get; set; }
        public Guid? TestTemplateTypeId { get; set; }
        public DateTime? availableFrom { get; set; }
        public DateTime? availableTo { get; set; }
        public int maxAttempts { get; set; }
        public TestStatus status { get; set; } 

        public virtual Lesson? Lesson { get; set; } = null!;
        public virtual User CreatedByUser { get; set; } = null!;
        public virtual TestTemplateType? TestTemplateType { get; set; } = null!;
        public virtual ICollection<TestQuestion> TestQuestions { get; set; } = new List<TestQuestion>();
        public virtual ICollection<TestAttempt> TestAttempts { get; set; } = new List<TestAttempt>();
        public virtual ICollection<StudyPlanItem> StudyPlanItems { get; set; } = new List<StudyPlanItem>();
        public virtual ICollection<TestScoreSummary> TestScoreSummaries { get; set; } = new List<TestScoreSummary>();
    }
}
