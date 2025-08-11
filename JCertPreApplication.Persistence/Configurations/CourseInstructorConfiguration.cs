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

            // Configure primary key - Use Id as single primary key
            builder.HasKey(ci => ci.Id);

            // Configure required properties
            builder.Property(ci => ci.CourseId).IsRequired();
            builder.Property(ci => ci.InstructorId).IsRequired();
            builder.Property(ci => ci.AssignedOn).IsRequired();
            builder.Property(ci => ci.IsActive).IsRequired();
            builder.Property(ci => ci.Notes).HasMaxLength(500);

            // Add index for common queries
            builder.HasIndex(ci => new { ci.CourseId, ci.IsActive })
                .HasDatabaseName("IX_CourseInstructor_CourseId_IsActive");

            builder.HasIndex(ci => new { ci.InstructorId, ci.IsActive })
                .HasDatabaseName("IX_CourseInstructor_InstructorId_IsActive");

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