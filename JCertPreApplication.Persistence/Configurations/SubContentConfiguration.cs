using JCertPreApplication.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JCertPreApplication.Persistence.Configurations
{
    public class SubContentConfiguration : IEntityTypeConfiguration<SubContent>
{
    public void Configure(EntityTypeBuilder<SubContent> builder)
    {
        builder.ToTable("sub_contents");
        builder.HasKey(sc => sc.SubContentId);

        builder.Property(sc => sc.SubContentName)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(100);

        builder.Property(sc => sc.Level)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(10);

        builder.Property(sc => sc.ContentName)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        // Configure relationships
        builder.HasMany(sc => sc.Questions)
            .WithOne(q => q.SubContent)
            .HasForeignKey(q => q.SubContentId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(sc => sc.TestTemplateConfigs)
            .WithOne(tc => tc.SubContent)
            .HasForeignKey(tc => tc.subContentId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
}