using JCertPreApplication.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace JCertPreApplication.Persistence.Configurations
{
    public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
    {
        public void Configure(EntityTypeBuilder<Conversation> builder)
        {
            // Configure primary key
            builder.ToTable("conversation");
            builder.HasKey(c => c.conversationId);

            // Configure properties
            builder.Property(c => c.conversationName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(c => c.createdAt)
                   .IsRequired();

            // Configure navigation properties
            builder.HasMany(c => c.Participants)
                   .WithMany(cp => cp.Conversations)
                   .UsingEntity(j => j.ToTable("conversation_participant"));

            builder.HasMany(c => c.Messages)
                   .WithOne(m => m.Conversation)
                   .HasForeignKey(m => m.conversationId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
