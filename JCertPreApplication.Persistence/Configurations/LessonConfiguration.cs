using JCertPreApplication.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JCertPreApplication.Persistence.Configurations
{
    public class LessonConfiguration : IEntityTypeConfiguration<Lesson>
    {
        public void Configure(EntityTypeBuilder<Lesson> builder)
        {
            builder.ToTable("lesson");
            builder.HasKey(l => l.lessonId);

            builder.Property(l => l.courseId).IsRequired();
            builder.Property(l => l.title).IsRequired().HasMaxLength(200);
            builder.Property(l => l.lessonOrder).IsRequired();
            builder.Property(l => l.content).IsRequired();
            builder.Property(l => l.comment).IsRequired(false);

            builder.HasOne(l => l.Course)
                .WithMany(c => c.Lessons)
                .HasForeignKey(l => l.courseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(l => l.Test)
                .WithOne(t => t.Lesson)
                .HasForeignKey<Test>(t => t.lessonId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(l => l.Documents)
                .WithOne(d => d.Lesson)
                .HasForeignKey(d => d.lessonId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(l => l.LessonProgresses)
                .WithOne(lp => lp.Lesson)
                .HasForeignKey(lp => lp.lessonId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(l => l.courseId);
            builder.HasIndex(l => l.title);
            builder.HasIndex(l => l.lessonOrder);
            builder.HasIndex(l => new { l.courseId, l.title });
            builder.HasIndex(l => new { l.courseId, l.lessonOrder });
        }
    }
}
