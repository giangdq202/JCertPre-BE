using JCertPreApplication.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace JCertPreApplication.Persistence.Configurations
{
    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            // Configure primary key
            builder.HasKey(m => m.messageId);

            // Configure required properties and constraints
            builder.Property(m => m.senderId).IsRequired();
            builder.Property(m => m.conversationId).IsRequired();
            builder.Property(m => m.content).IsRequired().HasMaxLength(1000);
            builder.Property(m => m.sentAt).IsRequired();

            // Configure foreign key relationships
            builder.HasOne(m => m.User)
                   .WithMany(c => c.Messages)
                   .HasForeignKey(m => m.senderId).OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(m => m.Conversation)
                   .WithMany(c => c.Messages)
                   .HasForeignKey(m => m.conversationId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
