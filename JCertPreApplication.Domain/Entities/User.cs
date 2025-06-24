using JCertPreApplication.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCertPreApplication.Domain.Entities
{
    public class User
    {
        [Key]
        public Guid userId { get; set; }

        [Required]
        [StringLength(100)]
        public string fullName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string email { get; set; }

        [Required]
        [StringLength(255)]
        public string passwordHash { get; set; }

        [StringLength(20)]
        public string phone { get; set; }

        [StringLength(500)]
        public string avatarUrl { get; set; }

        public int credit { get; set; }

        public DateTime createdAt { get; set; }

        public DateTime lastLogin { get; set; }

        public UserStatus status { get; set; }

        // Mối quan hệ nhiều-nhiều với Role qua UserRole
        public virtual ICollection<UserRole> UserRoles { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        // Quan hệ với InstructorProfile
        public virtual InstructorProfile InstructorProfile { get; set; }
        // Quan hệ với StudentProfile
        public virtual StudentProfile StudentProfile { get; set; }
        public virtual ICollection<Enrollment> Enrollments { get; set; }
        public virtual ICollection<Course> Courses { get; set; }

        // Quan hệ với ConversationParticipant
        public virtual ICollection<ConversationParticipant> ConversationParticipants { get; set; }
        // Quan hệ với Report (làm Student hoặc Instructor)
        public virtual ICollection<Report> StudentReports { get; set; }
        // Quan hệ với Message
        public virtual ICollection<Message> Messages { get; set; }
        // Quan hệ với StudyPlan
        public virtual ICollection<StudyPlan> StudentPlans { get; set; }
        // Quan hệ với TestAttempt
        public virtual ICollection<TestAttempt> TestAttempts { get; set; }
        public virtual ICollection<Test> CreatedTests { get; set; }
    }

}
