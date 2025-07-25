using JCertPreApplication.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace JCertPreApplication.Persistence.Configurations
{
    public class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            // Configure primary key
            builder.ToTable("course");
            builder.HasKey(c => c.courseId);

            // Configure required properties and constraints
            builder.Property(c => c.title).IsRequired().HasMaxLength(100);
            builder.Property(c => c.description).IsRequired().HasMaxLength(1000);
            builder.Property(c => c.level).IsRequired().HasConversion<string>();
            builder.Property(c => c.courseType).IsRequired().HasConversion<string>();
            builder.Property(c => c.price).HasPrecision(18, 2).IsRequired();
            builder.Property(c => c.thumbnailUrl).HasMaxLength(500);
            builder.Property(c => c.status).IsRequired().HasConversion<string>();
            builder.Property(c => c.createdAt).IsRequired();

            // Configure relationship with instructors through CourseInstructor
            builder.HasMany(c => c.CourseInstructors)
                   .WithOne(ci => ci.Course)
                   .HasForeignKey(ci => ci.CourseId)
                   .OnDelete(DeleteBehavior.NoAction);

            // Configure navigation properties
            builder.HasMany(c => c.Lessons)
                   .WithOne()
                   .HasForeignKey(l => l.courseId).OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(c => c.Feedbacks)
                   .WithOne()
                   .HasForeignKey(f => f.courseId).OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(c => c.Enrollments)
                   .WithOne()
                   .HasForeignKey(e => e.courseId).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(c => c.StudyPlanItems)
                .WithOne(spi => spi.Course)
                .IsRequired(false) // Nullable foreign key
                .HasForeignKey(spi => spi.courseId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
