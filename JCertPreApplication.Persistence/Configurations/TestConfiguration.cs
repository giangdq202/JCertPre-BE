using JCertPreApplication.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace JCertPreApplication.Persistence.Configurations
{
    public class TestConfiguration : IEntityTypeConfiguration<Test>
    {
        public void Configure(EntityTypeBuilder<Test> builder)
        {
            builder.ToTable("test");
            builder.HasKey(t => t.testId);

            builder.Property(t => t.title).IsRequired().HasMaxLength(100);
            builder.Property(t => t.description).IsRequired();
            builder.Property(t => t.testType).IsRequired().HasMaxLength(50);
            builder.Property(t => t.durationMinutes).IsRequired();
            builder.Property(t => t.lessonId);
            builder.Property(t => t.createdByUserId).IsRequired();

            builder.HasOne(t => t.Lesson)
                   .WithMany(q => q.Tests)
                   .IsRequired(false)
                   .HasForeignKey(t => t.lessonId).OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(t => t.CreatedByUser)
                   .WithMany(q => q.CreatedTests)
                   .HasForeignKey(t => t.createdByUserId).OnDelete(DeleteBehavior.NoAction);

            // Add new one-to-many for TestQuestion
            builder.HasMany(t => t.TestQuestions)
                   .WithOne(tq => tq.Test)
                   .HasForeignKey(tq => tq.testId);

            builder.HasMany(t => t.TestAttempts)
                   .WithOne(q => q.Test)
                   .HasForeignKey(ta => ta.testId).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(t => t.StudyPlanItems)
                   .WithOne(q => q.Test)
                   .IsRequired(false)
                   .HasForeignKey(ta => ta.testId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
