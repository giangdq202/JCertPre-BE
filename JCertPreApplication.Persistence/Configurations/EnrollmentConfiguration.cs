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
    public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
    {
        public void Configure(EntityTypeBuilder<Enrollment> builder)
        {
            // Configure primary key
            builder.HasKey(e => e.enrollmentId);

            // Configure required properties
            builder.Property(e => e.userId).IsRequired();
            builder.Property(e => e.courseId).IsRequired();
            builder.Property(e => e.enrollDate).IsRequired();
            builder.Property(e => e.price).IsRequired();

            // Configure foreign key relationships
            builder.HasOne(e => e.User)
                   .WithMany()
                   .HasForeignKey(e => e.userId);

            builder.HasOne(e => e.Course)
                   .WithMany(c => c.Enrollments)
                   .HasForeignKey(e => e.courseId);
        }
    }
}
