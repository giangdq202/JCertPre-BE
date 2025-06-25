using JCertPreApplication.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace JCertPreApplication.Persistence.Configurations
{
    public class AttemptAnswerConfiguration : IEntityTypeConfiguration<AttemptAnswer>
    {
        public void Configure(EntityTypeBuilder<AttemptAnswer> builder)
        {
            // Configure primary key
            builder.HasKey(aa => aa.answerId);

            // Configure required properties
            builder.Property(aa => aa.attemptId).IsRequired();
            builder.Property(aa => aa.questionId).IsRequired();
            builder.Property(aa => aa.choiceId).IsRequired();

            // Configure foreign key relationships
            builder.HasOne(aa => aa.TestAttempt)
                   .WithMany(ta => ta.AttemptAnswers)
                   .HasForeignKey(aa => aa.attemptId)
                    .OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(aa => aa.Question)
                   .WithMany(q => q.AttemptAnswers)
                   .HasForeignKey(aa => aa.questionId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(aa => aa.Choice)
                   .WithMany(c => c.AttemptAnswers)
                   .HasForeignKey(aa => aa.choiceId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
