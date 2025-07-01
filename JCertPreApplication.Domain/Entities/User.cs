using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.Domain.Entities
{
    public class User
    {
        public Guid userId { get; set; }
        public string fullName { get; set; }
        public string email { get; set; }
        public string passwordHash { get; set; }
        public string? phone { get; set; }
        public string? avatarUrl { get; set; }
        public int credit { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime lastLogin { get; set; }
        public UserStatus status { get; set; }

        public Guid roleId { get; set; }
        public Role Role { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual InstructorProfile InstructorProfile { get; set; }
        public virtual StudentProfile StudentProfile { get; set; }
        public virtual ICollection<Enrollment> Enrollments { get; set; }
        public virtual ICollection<Course> InstructorCourses { get; set; } = new List<Course>();
        public virtual ICollection<Conversation> Conversations { get; set; }
        public virtual ICollection<Report> StudentReports { get; set; } // For Report.StudentUser
        public virtual ICollection<Report> InstructorReports { get; set; } // For Report.InstructorUser
        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<StudyPlan> StudentPlans { get; set; }
        public virtual ICollection<StudyPlan> StaffCreatePlans { get; set; }
        public virtual ICollection<TestAttempt> TestAttempts { get; set; }
        public virtual ICollection<Test> CreatedTests { get; set; }
    }

}
