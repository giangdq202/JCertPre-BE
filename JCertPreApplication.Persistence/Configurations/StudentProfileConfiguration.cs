using JCertPreApplication.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace JCertPreApplication.Persistence.Configurations
{
    public class StudentProfileConfiguration : IEntityTypeConfiguration<StudentProfile>
    {
        public void Configure(EntityTypeBuilder<StudentProfile> builder)
        {
            // Configure primary key
            builder.ToTable("student_profile");
            builder.HasKey(sp => sp.userId);

            // Configure required properties and constraints
            builder.Property(sp => sp.currentLevel).IsRequired().HasMaxLength(50);
            builder.Property(sp => sp.learningGoals).IsRequired();

            // Configure foreign key relationship
            builder.HasOne(sp => sp.User)
                   .WithOne()
                   .HasForeignKey<StudentProfile>(sp => sp.userId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
