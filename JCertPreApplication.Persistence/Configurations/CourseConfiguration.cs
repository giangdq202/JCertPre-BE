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
    public class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            // Configure primary key
            builder.HasKey(c => c.courseId);

            // Configure required properties and constraints
            builder.Property(c => c.staffCreateUserId).IsRequired();
            builder.Property(c => c.title).IsRequired().HasMaxLength(100);
            builder.Property(c => c.description).IsRequired().HasMaxLength(1000);
            builder.Property(c => c.level).IsRequired();
            builder.Property(c => c.courseType).IsRequired();
            builder.Property(c => c.price).HasPrecision(18, 2).IsRequired();
            builder.Property(c => c.thumbnailUrl).IsRequired();
            builder.Property(c => c.status).IsRequired();
            builder.Property(c => c.createdAt).IsRequired();

            // Configure foreign key relationship
            builder.HasOne(c => c.User)
                   .WithMany()
                   .HasForeignKey(c => c.staffCreateUserId);

            // Configure navigation properties
            builder.HasMany(c => c.Lessons)
                   .WithOne()
                   .HasForeignKey(l => l.courseId);

            builder.HasMany(c => c.Livestreams)
                   .WithOne()
                   .HasForeignKey(ls => ls.courseId);


    

            builder.HasMany(c => c.Feedbacks)
                   .WithOne()
                   .HasForeignKey(f => f.courseId);

            builder.HasMany(c => c.Enrollments)
                   .WithOne()
                   .HasForeignKey(e => e.courseId);
        }
    }
}
