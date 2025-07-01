using JCertPreApplication.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace JCertPreApplication.Persistence.Configurations
{
    public class StudyPlanItemConfiguration : IEntityTypeConfiguration<StudyPlanItem>
    {
        public void Configure(EntityTypeBuilder<StudyPlanItem> builder)
        {
            // Configure primary key
            builder.ToTable("study_plan_item");
            builder.HasKey(spi => spi.itemId);

            // Configure required properties and constraints
            builder.Property(spi => spi.planId).IsRequired();
            builder.Property(spi => spi.sequence).IsRequired();
            builder.Property(spi => spi.itemType).IsRequired().HasMaxLength(50);
            builder.Property(spi => spi.itemIdRef).IsRequired();
            builder.Property(spi => spi.status).IsRequired();

            // Configure foreign key relationship
            builder.HasOne(spi => spi.StudyPlan)
                   .WithMany(sp => sp.StudyPlanItems)
                   .HasForeignKey(spi => spi.planId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
