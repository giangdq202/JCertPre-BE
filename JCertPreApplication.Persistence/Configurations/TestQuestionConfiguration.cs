using JCertPreApplication.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JCertPreApplication.Persistence.Configurations
{
    public class TestQuestionConfiguration : IEntityTypeConfiguration<TestQuestion>
    {
        public void Configure(EntityTypeBuilder<TestQuestion> builder)
        {
            builder.ToTable("test_question");
            builder.HasKey(tq => tq.testQuestionId);

            builder.Property(tq => tq.isActive)
                .IsRequired();

            builder.Property(tq => tq.questionNumber)
                .IsRequired();

            builder.Property(tq => tq.partNumber)
                .IsRequired(false);

            builder.Property(tq => tq.partDurationMinutes)
                .IsRequired(false);

            builder.HasOne(tq => tq.Test)
                .WithMany(t => t.TestQuestions)
                .HasForeignKey(tq => tq.testId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(tq => tq.Question)
                .WithMany(q => q.TestQuestions)
                .HasForeignKey(tq => tq.questionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}