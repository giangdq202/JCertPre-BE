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

            builder.Property(tss => tss.KanjiScore).HasColumnType("text").IsRequired(false);
            builder.Property(tss => tss.VocabularyScore).HasColumnType("text").IsRequired(false);
            builder.Property(tss => tss.GrammarScore).HasColumnType("text").IsRequired(false);
            builder.Property(tss => tss.ReadingScore).HasColumnType("text").IsRequired(false);
            builder.Property(tss => tss.ListeningScore).HasColumnType("text").IsRequired(false);

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