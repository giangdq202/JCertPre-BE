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
    public class TagConfiguration : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            // Configure primary key
            builder.HasKey(t => t.tagId);

            // Configure required properties and constraints
            builder.Property(t => t.tagLevel).IsRequired().HasMaxLength(50);
            builder.Property(t => t.contentSection).IsRequired().HasMaxLength(100);
            builder.Property(t => t.contentDetail).IsRequired().HasMaxLength(1000);
            builder.Property(t => t.tagScore).IsRequired();

            // Configure navigation property
            builder.HasMany(t => t.Questions)
               .WithMany(q => q.Tag)
               .UsingEntity(j => j.ToTable("QuestionTags"));
        }
    }
}
