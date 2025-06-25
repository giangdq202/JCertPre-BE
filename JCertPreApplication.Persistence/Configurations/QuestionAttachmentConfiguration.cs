using JCertPreApplication.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace JCertPreApplication.Persistence.Configurations
{
    public class QuestionAttachmentConfiguration : IEntityTypeConfiguration<QuestionAttachment>
    {
        public void Configure(EntityTypeBuilder<QuestionAttachment> builder)
        {
            // Configure primary key
            builder.HasKey(qa => qa.attachmentId);

            // Configure required properties and constraints
            builder.Property(qa => qa.questionId).IsRequired();
            builder.Property(qa => qa.mediaUrl).IsRequired().HasMaxLength(100);
            builder.Property(qa => qa.mediaType).IsRequired();

            // Configure foreign key relationship
            builder.HasOne(qa => qa.Question)
                   .WithMany(q => q.QuestionAttachments)
                   .HasForeignKey(qa => qa.questionId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
