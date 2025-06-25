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
    public class InstructorProfileConfiguration : IEntityTypeConfiguration<InstructorProfile>
    {
        public void Configure(EntityTypeBuilder<InstructorProfile> builder)
        {
            // Configure primary key
            builder.HasKey(ip => ip.userId);

            // Configure required properties and constraints
            builder.Property(ip => ip.introduction).IsRequired().HasMaxLength(1000);
            builder.Property(ip => ip.experience).HasMaxLength(255);
            builder.Property(ip => ip.teachingStyle).HasMaxLength(50);

            // Configure foreign key relationship
            builder.HasOne(ip => ip.User)
                   .WithOne()
                   .HasForeignKey<InstructorProfile>(ip => ip.userId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
