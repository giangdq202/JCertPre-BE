using JCertPreApplication.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCertPreApplication.Persistence.DatabaseContext
{
    public class JCertPreDatabaseContext: DbContext
    {
        public JCertPreDatabaseContext(DbContextOptions<JCertPreDatabaseContext> options)
            : base(options)
        {
        }

        // DbSet cho tất cả các entity
        public DbSet<AttemptAnswer> AttemptAnswers { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<ConversationParticipant> ConversationParticipants { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Choice> Choices { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<InstructorProfile> InstructorProfiles { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Livestream> Livestreams { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionAttachment> QuestionAttachments { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<StudentProfile> StudentProfiles { get; set; }
        public DbSet<StudyPlan> StudyPlans { get; set; }
        public DbSet<StudyPlanItem> StudyPlanItems { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<TestAttempt> TestAttempts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Additional model configurations can be done here
        }
    }
    
    
}
