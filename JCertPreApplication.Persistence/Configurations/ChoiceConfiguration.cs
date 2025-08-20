using JCertPreApplication.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace JCertPreApplication.Persistence.Configurations
{
    public class ChoiceConfiguration : IEntityTypeConfiguration<Choice>
    {
        public void Configure(EntityTypeBuilder<Choice> builder)
        {
            builder.ToTable("choice");
            builder.HasKey(c => c.choiceId);

            // Properties
            builder.Property(c => c.questionId)
                .IsRequired();

            builder.Property(c => c.choiceText)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(c => c.isCorrect)
                .IsRequired();

            // Relationships
            builder.HasOne(c => c.Question)
                   .WithMany(q => q.Choices)
                   .HasForeignKey(c => c.questionId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(c => c.AttemptAnswers)
                   .WithOne(aa => aa.Choice)
                   .HasForeignKey(aa => aa.choiceId)
                   .OnDelete(DeleteBehavior.NoAction);

            // Indexes for performance
            builder.HasIndex(c => c.questionId); // For fast lookup by question
            builder.HasIndex(c => c.isCorrect); // For correctness checks
            builder.HasIndex(c => new { c.questionId, c.isCorrect }); // For fast correct/incorrect choice lookup per question
        }
    }
}
