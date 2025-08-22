using JCertPreApplication.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace JCertPreApplication.Persistence.Configurations
{
    public class StudentProfileConfiguration : IEntityTypeConfiguration<StudentProfile>
    {
        public void Configure(EntityTypeBuilder<StudentProfile> builder)
        {
            // Table and primary key
            builder.ToTable("student_profile");
            builder.HasKey(sp => sp.userId);

            // Required properties and constraints
            builder.Property(sp => sp.currentLevel)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(sp => sp.learningGoals)
                .IsRequired();

            // New properties
            builder.Property(sp => sp.numberOfTestsTaken)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(sp => sp.lastResetTestTime)
                .IsRequired(false);

            // Foreign key relationship
            builder.HasOne(sp => sp.User)
                   .WithOne(u => u.StudentProfile)
                   .HasForeignKey<StudentProfile>(sp => sp.userId)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
