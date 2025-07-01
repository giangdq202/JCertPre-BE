using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.Domain.Entities
{
    public class Course
    {
        public Guid courseId { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public CourseLevel level { get; set; }
        public CourseType courseType { get; set; }
        public decimal price { get; set; }
        public string? thumbnailUrl { get; set; }
        public CourseStatus status { get; set; }
        public DateTime createdAt { get; set; }

        // Navigation properties
        public virtual ICollection<User> Instructors { get; set; } = new List<User>();
        public virtual ICollection<Lesson> Lessons { get; set; }
        public virtual ICollection<Livestream> Livestreams { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<Enrollment> Enrollments { get; set; }
    }
}
