using JCertPreApplication.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

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
                   .HasForeignKey(f => f.courseId).OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(f => f.User)
                   .WithMany(c => c.Feedbacks)
                   .HasForeignKey(f => f.userId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
