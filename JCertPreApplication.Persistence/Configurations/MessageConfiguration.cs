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
                   .WithMany()
                   .HasForeignKey(m => m.senderId);

            builder.HasOne(m => m.Conversation)
                   .WithMany(c => c.Messages)
                   .HasForeignKey(m => m.conversationId);
        }
    }
}
