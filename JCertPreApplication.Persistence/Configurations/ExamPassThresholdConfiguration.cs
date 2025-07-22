using JCertPreApplication.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JCertPreApplication.Persistence.Configurations
{
    public class ExamPassThresholdConfiguration : IEntityTypeConfiguration<ExamPassThreshold>
    {
        public void Configure(EntityTypeBuilder<ExamPassThreshold> builder)
        {
            builder.ToTable("exam_pass_threshold");
            builder.HasKey(ept => ept.ExamPassThresholdId);

            builder.Property(ept => ept.ExamPassThresholdId)
                   .IsRequired();

            builder.Property(ept => ept.UserId)
                   .IsRequired();

            builder.Property(ept => ept.LevelName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(ept => ept.TotalMaxScore)
                   .IsRequired();

            builder.Property(ept => ept.TotalPassingScore)
                   .IsRequired();

            builder.Property(ept => ept.LanguageKnowledgeMin)
                   .IsRequired();

            builder.Property(ept => ept.LanguageKnowledgeMax)
                   .IsRequired();

            builder.Property(ept => ept.ReadingMax)
                   .IsRequired();

            builder.Property(ept => ept.ReadingMin)
                   .IsRequired();

            builder.Property(ept => ept.ListeningMax)
                   .IsRequired();

            builder.Property(ept => ept.ListeningMin)
                   .IsRequired();

            builder.Property(ept => ept.Status)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(ept => ept.LastUpdatedBy)
                   .IsRequired();

            builder.Property(ept => ept.CreatedAt)
                   .IsRequired();

            builder.HasOne(ept => ept.User)
                   .WithMany(u => u.ExamPassThresholds)
                   .HasForeignKey(ept => ept.UserId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(ept => ept.Tests)
                   .WithOne(t => t.ExamPassThreshold)
                   .HasForeignKey(t => t.ExamPassThresholdId)
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}