using JCertPreApplication.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JCertPreApplication.Persistence.Configurations
{
    public class TestScoreSummaryConfiguration : IEntityTypeConfiguration<TestScoreSummary>
    {
        public void Configure(EntityTypeBuilder<TestScoreSummary> builder)
        {
            builder.ToTable("test_score_summary");
            builder.HasKey(tss => tss.TestScoreSummaryId);

            builder.Property(tss => tss.TestId)
                .IsRequired();

            builder.Property(tss => tss.TestAttemptId)
                .IsRequired(false);

            builder.Property(tss => tss.kanji_score)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(tss => tss.kanji_max_score)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(tss => tss.vocab_score)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(tss => tss.vocab_max_score)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(tss => tss.grammar_score)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(tss => tss.grammar_max_score)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(tss => tss.reading_score)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(tss => tss.reading_max_score)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(tss => tss.listening_score)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(tss => tss.listening_max_score)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(tss => tss.total_score)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(tss => tss.total_max_score)
                .IsRequired()
                .HasDefaultValue(0);

            builder.HasOne(tss => tss.Test)
                .WithMany(t => t.TestScoreSummaries)
                .HasForeignKey(tss => tss.TestId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(tss => tss.TestAttempt)
                .WithMany(ta => ta.TestScoreSummaries)
                .HasForeignKey(tss => tss.TestAttemptId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}