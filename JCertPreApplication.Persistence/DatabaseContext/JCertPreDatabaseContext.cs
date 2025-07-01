using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using Npgsql.NameTranslation;

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Additional model configurations can be done here
            modelBuilder.ApplyConfiguration(new ReportConfiguration());
            modelBuilder.ApplyConfiguration(new AttemptAnswerConfiguration());
            modelBuilder.ApplyConfiguration(new ConversationConfiguration());
            modelBuilder.ApplyConfiguration(new CourseConfiguration());
            modelBuilder.ApplyConfiguration(new ChoiceConfiguration());
            modelBuilder.ApplyConfiguration(new DocumentConfiguration());
            modelBuilder.ApplyConfiguration(new EnrollmentConfiguration());
            modelBuilder.ApplyConfiguration(new FeedbackConfiguration());
            modelBuilder.ApplyConfiguration(new InstructorProfileConfiguration());
            modelBuilder.ApplyConfiguration(new LessonConfiguration());
            modelBuilder.ApplyConfiguration(new LivestreamConfiguration());
            modelBuilder.ApplyConfiguration(new MessageConfiguration());
            modelBuilder.ApplyConfiguration(new PaymentConfiguration());
            modelBuilder.ApplyConfiguration(new QuestionConfiguration());
            modelBuilder.ApplyConfiguration(new QuestionAttachmentConfiguration());
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new StudentProfileConfiguration());
            modelBuilder.ApplyConfiguration(new StudyPlanConfiguration());
            modelBuilder.ApplyConfiguration(new StudyPlanItemConfiguration());
            modelBuilder.ApplyConfiguration(new TagConfiguration());
            modelBuilder.ApplyConfiguration(new TestConfiguration());
            modelBuilder.ApplyConfiguration(new TestAttemptConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
        }
    }
    
    
    
}
