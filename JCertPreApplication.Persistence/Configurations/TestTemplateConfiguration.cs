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

            builder.Property(tt => tt.TestTemplateTypeId)
                .IsRequired();

            builder.Property(tt => tt.templateName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(tt => tt.durationMinutes)
                .IsRequired();

            builder.Property(tt => tt.totalScore)
                .IsRequired();

            builder.Property(tt => tt.toPassPercentage)
                .HasColumnType("decimal(5,2)")
                .IsRequired();

            builder.Property(tt => tt.sequence)
                .IsRequired();

            builder.HasOne(tt => tt.TestTemplateType)
                .WithMany(ttt => ttt.TestTemplates)
                .HasForeignKey(tt => tt.TestTemplateTypeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(tt => tt.TestTemplateConfigs)
                .WithOne(tc => tc.TestTemplate)
                .HasForeignKey(tc => tc.templateId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}