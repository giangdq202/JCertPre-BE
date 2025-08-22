using JCertPreApplication.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JCertPreApplication.Persistence.Configurations
{
    public class TestConfiguration : IEntityTypeConfiguration<Test>
    {
        public void Configure(EntityTypeBuilder<Test> builder)
        {
            builder.ToTable("test");
            builder.HasKey(t => t.testId);

            builder.Property(t => t.title)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.description)
                .IsRequired();

            builder.Property(t => t.testType)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(t => t.courseLevel)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(t => t.durationMinutes)
                .IsRequired();

            builder.Property(t => t.lessonId)
                .IsRequired(false);

            builder.Property(t => t.createdByUserId)
                .IsRequired();

            builder.Property(t => t.TestTemplateTypeId)
                .IsRequired(false);

            builder.Property(t => t.availableFrom)
                .IsRequired(false);

            builder.Property(t => t.availableTo)
                .IsRequired(false);

            builder.Property(t => t.maxAttempts)
                .IsRequired();

            builder.Property(t => t.passing_percentage)
                .HasColumnType("decimal(5,2)")
                .IsRequired()
                .HasDefaultValue(0m);

            builder.Property(t => t.status)
                .IsRequired()
                .HasConversion<string>();

            builder.HasOne(t => t.Lesson)
                .WithOne(l => l.Test)
                .HasForeignKey<Test>(t => t.lessonId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(t => t.CreatedByUser)
                .WithMany(u => u.CreatedTests)
                .HasForeignKey(t => t.createdByUserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(t => t.TestTemplateType)
                .WithMany(ttt => ttt.Tests)
                .HasForeignKey(t => t.TestTemplateTypeId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(t => t.TestQuestions)
                .WithOne(tq => tq.Test)
                .HasForeignKey(tq => tq.testId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(t => t.TestAttempts)
                .WithOne(ta => ta.Test)
                .HasForeignKey(ta => ta.testId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(t => t.TestScoreSummaries)
                .WithOne(tss => tss.Test)
                .HasForeignKey(tss => tss.TestId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes for performance
            builder.HasIndex(t => t.TestTemplateTypeId);
            builder.HasIndex(t => t.lessonId);
            builder.HasIndex(t => t.createdByUserId);
            builder.HasIndex(t => t.status);
            builder.HasIndex(t => t.testType);
            builder.HasIndex(t => t.courseLevel);
            builder.HasIndex(t => t.availableFrom);
            builder.HasIndex(t => new { t.TestTemplateTypeId, t.status });
        }
    }
}
