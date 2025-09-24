using JCertPreApplication.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace JCertPreApplication.Persistence.Configurations
{
    public class AttemptAnswerConfiguration : IEntityTypeConfiguration<AttemptAnswer>
    {
        public void Configure(EntityTypeBuilder<AttemptAnswer> builder)
        {
            builder.ToTable("attempt_answer");
            builder.HasKey(aa => aa.answerId);

            builder.Property(aa => aa.attemptId).IsRequired();
            builder.Property(aa => aa.questionId).IsRequired(false); // Now nullable
            builder.Property(aa => aa.choiceId).IsRequired(false);   // Now nullable

            builder.Property(aa => aa.WrittenAnswer)
                   .HasMaxLength(2000)
                   .IsRequired(false);

            builder.Property(aa => aa.GraderComment)
                   .HasMaxLength(1000)
                   .IsRequired(false);

            builder.Property(aa => aa.isCorrect)
                   .IsRequired();

            builder.Property(aa => aa.score)
                   .IsRequired();

            builder.HasOne(aa => aa.TestAttempt)
                   .WithMany(ta => ta.AttemptAnswers)
                   .HasForeignKey(aa => aa.attemptId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(aa => aa.Question)
                   .WithMany(q => q.AttemptAnswers)
                   .HasForeignKey(aa => aa.questionId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(aa => aa.Choice)
                   .WithMany(c => c.AttemptAnswers)
                   .HasForeignKey(aa => aa.choiceId)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
