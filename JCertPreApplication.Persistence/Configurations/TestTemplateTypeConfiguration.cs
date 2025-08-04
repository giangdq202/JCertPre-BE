using JCertPreApplication.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JCertPreApplication.Persistence.Configurations
{
    public class TestTemplateTypeConfiguration : IEntityTypeConfiguration<TestTemplateType>
    {
        public void Configure(EntityTypeBuilder<TestTemplateType> builder)
        {
            builder.ToTable("test_template_type");
            builder.HasKey(t => t.TestTemplateTypeId);

            builder.Property(t => t.userId)
                .IsRequired();

            builder.Property(t => t.typeName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.courseLevel)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(t => t.testType)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(t => t.description)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(t => t.isActive)
                .IsRequired();

            builder.Property(t => t.createdAt)
                .IsRequired();

            builder.Property(x => x.totalTestScore)
                .IsRequired();

            builder.Property(x => x.totalPassPercentage)
                .IsRequired();

            builder.HasOne(t => t.CreatedByUser)
                .WithMany(u => u.TestTemplateTypes)
                .HasForeignKey(t => t.userId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(t => t.TestTemplates)
                .WithOne(tt => tt.TestTemplateType)
                .HasForeignKey(tt => tt.TestTemplateTypeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(t => t.Tests)
                .WithOne(test => test.TestTemplateType)
                .HasForeignKey(test => test.TestTemplateTypeId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}