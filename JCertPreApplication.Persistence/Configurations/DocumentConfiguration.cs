using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JCertPreApplication.Domain.Entities;

namespace JCertPreApplication.Persistence.Configurations
{
    public class DocumentConfiguration : IEntityTypeConfiguration<Document>
    {
        public void Configure(EntityTypeBuilder<Document> builder)
        {
            // Configure primary key
            builder.HasKey(d => d.documentId);

            // Configure required properties and constraints
            builder.Property(d => d.lessonId).IsRequired();
            builder.Property(d => d.documentName).IsRequired().HasMaxLength(100);
            builder.Property(d => d.fileUrl).IsRequired();
            builder.Property(d => d.uploadedAt).IsRequired();

            // Configure foreign key relationship
            builder.HasOne(d => d.Lesson)
                   .WithMany(l => l.Documents)
                   .HasForeignKey(d => d.lessonId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
