using JCertPreApplication.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace JCertPreApplication.Persistence.Configurations
{
    public class QuestionConfiguration : IEntityTypeConfiguration<Question>
    {
        public void Configure(EntityTypeBuilder<Question> builder)
        {
            // Configure primary key
            builder.ToTable("question");
            builder.HasKey(q => q.questionId);

            // Configure required properties and constraints
            builder.Property(q => q.questionText).IsRequired();
            builder.Property(q => q.questionType).IsRequired().HasMaxLength(50);
            builder.Property(q => q.explanation).IsRequired();
            builder.Property(q => q.tagId).IsRequired();

            // Configure foreign key relationship (one-to-many)
            builder.HasOne(q => q.Tag)
                   .WithMany(t => t.Questions)
                   .HasForeignKey(q => q.tagId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Configure navigation properties
            builder.HasMany(q => q.Tests)
                   .WithMany(t => t.Questions)
                   .UsingEntity(j => j.ToTable("question_test"));

            builder.HasMany(q => q.Choices)
                   .WithOne(c => c.Question)
                   .HasForeignKey(c => c.questionId).OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(q => q.QuestionAttachments)
                   .WithOne()
                   .HasForeignKey(qa => qa.questionId).OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(q => q.AttemptAnswers)
                   .WithOne(aa => aa.Question)
                   .HasForeignKey(aa => aa.questionId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
