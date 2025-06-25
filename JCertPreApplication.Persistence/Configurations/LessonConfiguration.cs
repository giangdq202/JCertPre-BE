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
    public class LessonConfiguration : IEntityTypeConfiguration<Lesson>
    {
        public void Configure(EntityTypeBuilder<Lesson> builder)
        {
            // Configure primary key
            builder.HasKey(l => l.lessonId);

            // Configure required properties and constraints
            builder.Property(l => l.courseId).IsRequired();
            builder.Property(l => l.title).IsRequired().HasMaxLength(100);
            builder.Property(l => l.lessonOrder).IsRequired();
            builder.Property(l => l.content).IsRequired();

            // Configure foreign key relationship
            builder.HasOne(l => l.Course)
                   .WithMany(c => c.Lessons)
                   .HasForeignKey(l => l.courseId);

            // Configure navigation properties
            builder.HasMany(l => l.Documents)
                   .WithOne(d => d.Lesson)
                   .HasForeignKey(d => d.lessonId);

            builder.HasMany(l => l.Tests)
                   .WithOne()
                   .HasForeignKey(t => t.lessonId);
        }
    }
}
