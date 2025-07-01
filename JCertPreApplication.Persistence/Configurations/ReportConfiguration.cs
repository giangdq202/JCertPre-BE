using JCertPreApplication.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace JCertPreApplication.Persistence.Configurations
{
    public class ReportConfiguration : IEntityTypeConfiguration<Report>
    {
        public void Configure(EntityTypeBuilder<Report> builder)
        {
            // Configure primary key
            builder.ToTable("report");
            builder.HasKey(r => r.reportId);

            // Configure required properties and constraints
            builder.Property(r => r.reporterStudentId).IsRequired();
            builder.Property(r => r.reportedInstructorId).IsRequired();
            builder.Property(r => r.reportContent).IsRequired().HasMaxLength(1000);
            builder.Property(r => r.status).IsRequired();
            builder.Property(r => r.createdAt).IsRequired();

            // Configure foreign key relationships
            builder.HasOne(r => r.StudentUser)
                   .WithMany(u => u.StudentReports)
                   .HasForeignKey(r => r.reporterStudentId)
                    .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(r => r.InstructorUser)
                   .WithMany(u => u.InstructorReports)
                   .HasForeignKey(r => r.reportedInstructorId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
