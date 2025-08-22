using JCertPreApplication.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace JCertPreApplication.Persistence.Configurations
{
    public class StudyPlanItemConfiguration : IEntityTypeConfiguration<StudyPlanItem>
    {
        public void Configure(EntityTypeBuilder<StudyPlanItem> builder)
        {
            builder.ToTable("study_plan_item");
            builder.HasKey(spi => spi.itemId);

            builder.Property(spi => spi.planId).IsRequired();
            builder.Property(spi => spi.sequence).IsRequired();
            builder.Property(spi => spi.itemType).IsRequired().HasMaxLength(50);
            builder.Property(spi => spi.courseId).IsRequired(false);
            builder.Property(spi => spi.TestTemplateTypeId).IsRequired(false);
            builder.Property(spi => spi.description).HasMaxLength(1000).IsRequired(false);
            builder.Property(spi => spi.status).IsRequired().HasConversion<string>();

            builder.HasOne(spi => spi.StudyPlan)
                   .WithMany(sp => sp.StudyPlanItems)
                   .HasForeignKey(spi => spi.planId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(spi => spi.Course)
                   .WithMany(sp => sp.StudyPlanItems)
                   .HasForeignKey(spi => spi.courseId)
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(spi => spi.TestTemplateType)
                   .WithMany()
                   .HasForeignKey(spi => spi.TestTemplateTypeId)
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
