using JCertPreApplication.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCertPreApplication.Persistence.Configurations
{
    public class ConversationParticipantConfiguration : IEntityTypeConfiguration<ConversationParticipant>
    {
        public void Configure(EntityTypeBuilder<ConversationParticipant> builder)
        {
            // Configure composite primary key
            builder.HasKey(cp => new { cp.conversationId, cp.userId });

            // Configure column order
            builder.Property(cp => cp.conversationId).HasColumnOrder(1);
            builder.Property(cp => cp.userId).HasColumnOrder(2);

            // Configure required properties
            builder.Property(cp => cp.joinedAt).IsRequired();

            // Configure foreign key relationships
            builder.HasOne(cp => cp.Conversation)
                   .WithMany(c => c.Participants)
                   .HasForeignKey(cp => cp.conversationId);

            builder.HasOne(cp => cp.User)
                   .WithMany()
                   .HasForeignKey(cp => cp.userId);
        }
    }
}
