using JCertPreApplication.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JCertPreApplication.Persistence.Configurations
{
    public class TestTemplateConfiguration : IEntityTypeConfiguration<TestTemplate>
    {
        public void Configure(EntityTypeBuilder<TestTemplate> builder)
        {
            builder.ToTable("test_template");
            builder.HasKey(tt => tt.templateId);

            builder.Property(tt => tt.templateName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(tt => tt.userId)
                .IsRequired();

            builder.Property(tt => tt.courseLevel)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(tt => tt.testType)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(tt => tt.durationMinutes)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(tt => tt.description)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(tt => tt.threeFirstParts)
                .HasColumnType("text")
                .IsRequired(false);

            builder.Property(tt => tt.fourFirstParts)
                .HasColumnType("text")
                .IsRequired(false);

            builder.Property(tt => tt.reading)
                .HasColumnType("text")
                .IsRequired(false);

            builder.Property(tt => tt.listening)
                .IsRequired()
                .HasColumnType("text");

            builder.Property(tt => tt.total)
                .IsRequired()
                .HasColumnType("text");

            builder.Property(tt => tt.isActive)
                .IsRequired();

            builder.Property(tt => tt.createdAt)
                .IsRequired();

            builder.HasOne(tt => tt.CreatedByUser)
                .WithMany(u => u.TestTemplates)
                .HasForeignKey(tt => tt.userId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(tt => tt.TestTemplateConfigs)
                .WithOne(tc => tc.TestTemplate)
                .HasForeignKey(tc => tc.templateId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(tt => tt.Tests)
                .WithOne(t => t.TestTemplate)
                .HasForeignKey(t => t.testTemplateId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}