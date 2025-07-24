using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.Domain.Entities
{
    public class User
    {
        public Guid userId { get; set; }
        public string fullName { get; set; } = null!;
        public string email { get; set; } = null!;
        public string passwordHash { get; set; } = null!;
        public string? phone { get; set; }
        public string? avatarUrl { get; set; }
        public int credit { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime lastLogin { get; set; }
        public UserStatus status { get; set; }

        public Guid roleId { get; set; }
        public Role Role { get; set; } = null!;
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
        public virtual InstructorProfile InstructorProfile { get; set; } = null!;
        public virtual StudentProfile StudentProfile { get; set; } = null!;
        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public virtual ICollection<Conversation> Conversations { get; set; } = new List<Conversation>();
        public virtual ICollection<Report> StudentReports { get; set; } = new List<Report>(); // For Report.StudentUser
        public virtual ICollection<Report> InstructorReports { get; set; } = new List<Report>(); // For Report.InstructorUser
        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
        public virtual ICollection<StudyPlan> StudentPlans { get; set; } = new List<StudyPlan>();
        public virtual ICollection<StudyPlan> StaffCreatePlans { get; set; } = new List<StudyPlan>();
        public virtual ICollection<TestAttempt> TestAttempts { get; set; } = new List<TestAttempt>();
        public virtual ICollection<Test> CreatedTests { get; set; } = new List<Test>();
        public virtual ICollection<CourseInstructor> InstructorCourses { get; set; } = new List<CourseInstructor>();
        public virtual ICollection<LessonProgress> LessonProgresses { get; set; } = new List<LessonProgress>();
        public virtual ICollection<TestTemplate> TestTemplates { get; set; } = new List<TestTemplate>();
    }
}
