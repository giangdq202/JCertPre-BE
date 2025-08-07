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
                .WithMany(l => l.Tests)
                .HasForeignKey(t => t.lessonId)
                .IsRequired(false)
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
                .HasForeignKey(tq => tq.testId);

            builder.HasMany(t => t.TestAttempts)
                .WithOne(ta => ta.Test)
                .HasForeignKey(ta => ta.testId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(t => t.StudyPlanItems)
                .WithOne(spi => spi.Test)
                .HasForeignKey(spi => spi.testId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(t => t.TestScoreSummaries)
                .WithOne(tss => tss.Test)
                .HasForeignKey(tss => tss.TestId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
