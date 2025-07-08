using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JCertPreApplication.Persistence.Configurations
{
    public class SubContentConfiguration : IEntityTypeConfiguration<SubContent>
    {
        public void Configure(EntityTypeBuilder<SubContent> builder)
        {
            builder.ToTable("SubContents");
            builder.HasKey(sc => sc.SubContentId);

            builder.Property(sc => sc.SubContentName)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.HasOne(sc => sc.Content)
                .WithMany(c => c.SubContents)
                .HasForeignKey(sc => sc.ContentId);
        }
    }
}