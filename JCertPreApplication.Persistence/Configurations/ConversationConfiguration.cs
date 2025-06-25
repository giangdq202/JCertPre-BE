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
    public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
    {
        public void Configure(EntityTypeBuilder<Conversation> builder)
        {
            // Configure primary key
            builder.HasKey(c => c.conversationId);

            // Configure properties
            builder.Property(c => c.conversationName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(c => c.createdAt)
                   .IsRequired();

            // Configure navigation properties
            builder.HasMany(c => c.Participants)
                   .WithOne()
                   .HasForeignKey(cp => cp.conversationId);

            builder.HasMany(c => c.Messages)
                   .WithOne()
                   .HasForeignKey(m => m.conversationId);
        }
    }
}
