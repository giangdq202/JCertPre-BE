using JCertPreApplication.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace JCertPreApplication.Persistence.Configurations
{
    public class QuestionConfiguration : IEntityTypeConfiguration<Question>
    {
        public void Configure(EntityTypeBuilder<Question> builder)
        {
            builder.ToTable("Questions");
            builder.HasKey(q => q.questionId);
            builder.Property(q => q.questionText).IsRequired();
            builder.Property(q => q.questionType).IsRequired();
            builder.Property(q => q.explanation).IsRequired();
            builder.Property(q => q.points);
            builder.Property(q => q.GUID).HasMaxLength(36);

            builder.HasOne(q => q.Level)
                .WithMany(l => l.Questions)
                .HasForeignKey(q => q.LevelId);

            builder.HasOne(q => q.Content)
                .WithMany(c => c.Questions)
                .HasForeignKey(q => q.ContentId);

            builder.HasOne(q => q.SubContent)
                .WithMany(sc => sc.Questions)
                .HasForeignKey(q => q.SubContentId);

            // Existing navigation properties
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
