using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JCertPreApplication.Persistence.Configurations
{
    public class TestAttemptConfiguration : IEntityTypeConfiguration<TestAttempt>
    {
        public void Configure(EntityTypeBuilder<TestAttempt> builder)
        {
            builder.ToTable("test_attempt");
            builder.HasKey(ta => ta.attemptId);

            builder.Property(ta => ta.userId)
                .IsRequired();

            builder.Property(ta => ta.testId)
                .IsRequired();

            builder.Property(ta => ta.startTime)
                .IsRequired();

            builder.Property(ta => ta.endTime)
                .IsRequired();

            builder.Property(ta => ta.attemptNumber)
                .IsRequired();

            builder.Property(ta => ta.status)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(ta => ta.totalScore)
                .IsRequired(false);

            builder.Property(ta => ta.languageKnowledgeScore)
                .IsRequired(false);

            builder.Property(ta => ta.readingScore)
                .IsRequired(false);

            builder.Property(ta => ta.listeningScore)
                .IsRequired(false);

            builder.Property(ta => ta.isPass)
                .IsRequired(false);

            builder.HasOne(ta => ta.User)
                .WithMany(u => u.TestAttempts)
                .HasForeignKey(ta => ta.userId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(ta => ta.Test)
                .WithMany(t => t.TestAttempts)
                .HasForeignKey(ta => ta.testId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(ta => ta.AttemptAnswers)
                .WithOne(aa => aa.TestAttempt)
                .HasForeignKey(aa => aa.attemptId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}