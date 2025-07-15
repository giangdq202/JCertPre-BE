using JCertPreApplication.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JCertPreApplication.Persistence.Configurations
{
    public class ExamPassThresholdConfiguration : IEntityTypeConfiguration<ExamPassThreshold>
    {
        public void Configure(EntityTypeBuilder<ExamPassThreshold> builder)
        {
            // Configure table name
            builder.ToTable("exam_pass_threshold");

            // Configure primary key
            builder.HasKey(ept => ept.ExamPassThresholdId);

            // Configure required properties and constraints
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

            // Configure foreign key relationship with User
            builder.HasOne(ept => ept.User)
                   .WithMany(u => u.ExamPassThresholds)
                   .HasForeignKey(ept => ept.UserId)
                   .OnDelete(DeleteBehavior.NoAction);

            

           
        }
    }
}