using JCertPreApplication.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JCertPreApplication.Persistence.Configurations
{
    public class ContentConfiguration : IEntityTypeConfiguration<Content>
    {
        public void Configure(EntityTypeBuilder<Content> builder)
        {
            builder.ToTable("Contents");
            builder.HasKey(c => c.ContentId);
            builder.Property(c => c.ContentName)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);
        }
    }
}