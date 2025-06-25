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
    public class FeedbackConfiguration : IEntityTypeConfiguration<Feedback>
    {
        public void Configure(EntityTypeBuilder<Feedback> builder)
        {
            // Configure primary key
            builder.HasKey(f => f.feedbackId);

            // Configure required properties
            builder.Property(f => f.courseId).IsRequired();
            builder.Property(f => f.userId).IsRequired();
            builder.Property(f => f.rating).IsRequired();
            builder.Property(f => f.comment).IsRequired();
            builder.Property(f => f.reply).IsRequired();
            builder.Property(f => f.createdAt).IsRequired();

            // Configure foreign key relationships
            builder.HasOne(f => f.Course)
                   .WithMany(c => c.Feedbacks)
                   .HasForeignKey(f => f.courseId);

            builder.HasOne(f => f.User)
                   .WithMany(c => c.Feedbacks)
                   .HasForeignKey(f => f.userId);
        }
    }
}
