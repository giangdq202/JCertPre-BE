using JCertPreApplication.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCertPreApplication.Persistence.Configurations
{
    public class StudyPlanItemConfiguration : IEntityTypeConfiguration<StudyPlanItem>
    {
        public void Configure(EntityTypeBuilder<StudyPlanItem> builder)
        {
            // Configure primary key
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
