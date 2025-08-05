using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.Domain.Entities
{
    public class Course
    {
        public Guid courseId { get; set; }
        public string title { get; set; } = null!;
        public string description { get; set; } = null!;
        public CourseLevel level { get; set; }
        public CourseType courseType { get; set; }
        public decimal price { get; set; }
        public string? thumbnailUrl { get; set; }
        public CourseStatus status { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime startDate { get; set; }  // New field
        public DateTime endDate { get; set; }    // New field

        // Navigation properties
        public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
        public virtual ICollection<Livestream> Livestreams { get; set; } = new List<Livestream>();
        public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public virtual ICollection<StudyPlanItem> StudyPlanItems { get; set; } = new List<StudyPlanItem>();
        public virtual ICollection<CourseInstructor> CourseInstructors { get; set; } = new List<CourseInstructor>();
    }
}
