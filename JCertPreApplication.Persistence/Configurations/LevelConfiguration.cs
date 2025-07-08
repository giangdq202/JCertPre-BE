using JCertPreApplication.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JCertPreApplication.Persistence.Configurations
{
    public class LevelConfiguration : IEntityTypeConfiguration<Level>
    {
        public void Configure(EntityTypeBuilder<Level> builder)
        {
            builder.ToTable("Levels");
            builder.HasKey(l => l.LevelId);
            builder.Property(l => l.LevelName)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(10);
        }
    }
}