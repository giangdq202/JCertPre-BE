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
            builder.ToTable("feedback");
            builder.HasKey(f => f.feedbackId);

            // Required properties
            builder.Property(f => f.courseId).IsRequired();
            builder.Property(f => f.userId).IsRequired();
            builder.Property(f => f.rating).IsRequired();
            builder.Property(f => f.createdAt).IsRequired();

            // Nullable property
            builder.Property(f => f.comment).IsRequired(false);

            // Foreign key relationships
            builder.HasOne(f => f.Course)
                   .WithMany(c => c.Feedbacks)
                   .HasForeignKey(f => f.courseId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(f => f.User)
                   .WithMany(u => u.Feedbacks)
                   .HasForeignKey(f => f.userId)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
