using JCertPreApplication.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JCertPreApplication.Persistence.Configurations
{
    public class LessonProgressConfiguration : IEntityTypeConfiguration<LessonProgress>
    {
        public void Configure(EntityTypeBuilder<LessonProgress> builder)
        {
            builder.ToTable("lesson_progress");
            builder.HasKey(lp => lp.progressId);

            builder.Property(lp => lp.progressId)
                .IsRequired();

            builder.Property(lp => lp.userId)
                .IsRequired();

            builder.Property(lp => lp.lessonId)
                .IsRequired();

            builder.Property(lp => lp.completionRate)
                .IsRequired()
                .HasColumnType("decimal(5,2)")
                .HasDefaultValue(0.0m);

            builder.HasOne(lp => lp.User)
                .WithMany(u => u.LessonProgresses)
                .HasForeignKey(lp => lp.userId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(lp => lp.Lesson)
                .WithMany(l => l.LessonProgresses)
                .HasForeignKey(lp => lp.lessonId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}