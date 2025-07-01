using JCertPreApplication.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace JCertPreApplication.Persistence.Configurations
{
    public class TestConfiguration : IEntityTypeConfiguration<Test>
    {
        public void Configure(EntityTypeBuilder<Test> builder)
        {
            // Configure primary key
            builder.ToTable("test");
            builder.HasKey(t => t.testId);

            // Configure required properties and constraints
            builder.Property(t => t.title).IsRequired().HasMaxLength(100);
            builder.Property(t => t.description).IsRequired();
            builder.Property(t => t.testType).IsRequired().HasMaxLength(50);
            builder.Property(t => t.durationMinutes).IsRequired();
            builder.Property(t => t.lessonId).IsRequired();
            builder.Property(t => t.createdByUserId).IsRequired();

            // Configure navigation properties
            builder.HasOne(t => t.Lesson)
                   .WithMany(q => q.Tests)
                   .HasForeignKey(t => t.lessonId).OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(t => t.CreatedByUser)
                   .WithMany(q => q.CreatedTests)
                   .HasForeignKey(t => t.createdByUserId).OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(t => t.Questions)
               .WithMany(q => q.Tests) // Đồng bộ với QuestionConfiguration
               .UsingEntity(j => j.ToTable("question_test"));

            builder.HasMany(t => t.TestAttempts)
                   .WithOne(q => q.Test)
                   .HasForeignKey(ta => ta.testId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
