using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using JCertPreApplication.Domain.Entities;

namespace JCertPreApplication.Persistence.Configurations
{
    public class CourseInstructorConfiguration : IEntityTypeConfiguration<CourseInstructor>
    {
        public void Configure(EntityTypeBuilder<CourseInstructor> builder)
        {
            builder.ToTable("course_instructor");

            // Configure composite primary key
            builder.HasKey(ci => new { ci.CourseId, ci.InstructorId, ci.AssignedOn });

            // Configure required properties
            builder.Property(ci => ci.AssignedOn).IsRequired();
            builder.Property(ci => ci.IsActive).IsRequired();
            builder.Property(ci => ci.Notes).HasMaxLength(500);

            // Configure relationships
            builder.HasOne(ci => ci.Course)
                .WithMany(c => c.CourseInstructors)
                .HasForeignKey(ci => ci.CourseId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(ci => ci.Instructor)
                .WithMany(i => i.InstructorCourses)
                .HasForeignKey(ci => ci.InstructorId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
} 