using JCertPreApplication.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JCertPreApplication.Persistence.Configurations
{
    public class TestTemplateConfigConfiguration : IEntityTypeConfiguration<TestTemplateConfig>
    {
        public void Configure(EntityTypeBuilder<TestTemplateConfig> builder)
        {
            builder.ToTable("test_template_config");
            builder.HasKey(tc => tc.configId);

            builder.Property(tc => tc.templateId)
                .IsRequired();

            builder.Property(tc => tc.subContentId)
                .IsRequired();

            builder.Property(tc => tc.questionCount)
                .IsRequired();

            builder.Property(tc => tc.pointPerQuestion)
                .IsRequired();

            builder.Property(tc => tc.totalPoints)
                .IsRequired();

            builder.Property(tc => tc.sequence)
                .IsRequired();

            builder.HasOne(tc => tc.TestTemplate)
                .WithMany(tt => tt.TestTemplateConfigs)
                .HasForeignKey(tc => tc.templateId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(tc => tc.SubContent)
                .WithMany(sc => sc.TestTemplateConfigs)
                .HasForeignKey(tc => tc.subContentId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}