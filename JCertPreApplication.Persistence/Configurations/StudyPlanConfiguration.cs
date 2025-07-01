using JCertPreApplication.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace JCertPreApplication.Persistence.Configurations
{
    public class StudyPlanConfiguration : IEntityTypeConfiguration<StudyPlan>
    {
        public void Configure(EntityTypeBuilder<StudyPlan> builder)
        {
            // Configure primary key
            builder.ToTable("study_plan");
            builder.HasKey(sp => sp.planId);

            // Configure required properties and constraints
            builder.Property(sp => sp.studentId).IsRequired();
            builder.Property(sp => sp.createdByStaffId).IsRequired();
            builder.Property(sp => sp.planName).IsRequired().HasMaxLength(100);
            builder.Property(sp => sp.description).IsRequired().HasMaxLength(1000);
            builder.Property(sp => sp.startDate).IsRequired();
            builder.Property(sp => sp.endDate).IsRequired();

            // Configure foreign key relationships
            builder.HasOne(sp => sp.Student)
                   .WithMany(st => st.StudentPlans)
                   .HasForeignKey(sp => sp.studentId).OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(sp => sp.Staff)
                   .WithMany(st => st.StaffCreatePlans)
                   .HasForeignKey(sp => sp.createdByStaffId).OnDelete(DeleteBehavior.NoAction);

            // Configure navigation property
            builder.HasMany(sp => sp.StudyPlanItems)
                   .WithOne(st => st.StudyPlan)
                   .HasForeignKey(st => st.planId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
