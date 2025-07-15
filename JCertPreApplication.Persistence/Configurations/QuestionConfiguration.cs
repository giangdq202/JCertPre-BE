using JCertPreApplication.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JCertPreApplication.Persistence.Configurations
{
    public class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.ToTable("questions");
        builder.HasKey(q => q.questionId);

        builder.Property(q => q.SubContentId)
            .IsRequired();

        builder.Property(q => q.questionText)
            .IsRequired();

        builder.Property(q => q.questionType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(q => q.explanation)
            .IsRequired();

        builder.Property(q => q.difficulty)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(10);

            builder.Property(q => q.points)
            .IsRequired();

        builder.HasOne(q => q.SubContent)
            .WithMany(sc => sc.Questions)
            .HasForeignKey(q => q.SubContentId)
            .IsRequired();

        // Add new one-to-many for TestQuestion
        builder.HasMany(q => q.TestQuestions)
            .WithOne(tq => tq.Question)
            .HasForeignKey(tq => tq.questionId);

        builder.HasMany(q => q.Choices)
            .WithOne(c => c.Question)
            .HasForeignKey(c => c.questionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(q => q.QuestionAttachments)
            .WithOne(qa => qa.Question)
            .HasForeignKey(qa => qa.questionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(q => q.AttemptAnswers)
            .WithOne(aa => aa.Question)
            .HasForeignKey(aa => aa.questionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
}
