using JCertPreApplication.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace JCertPreApplication.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // Configure primary key
            builder.ToTable("user");
            builder.HasKey(u => u.userId);

            // Configure required properties and constraints
            builder.Property(u => u.fullName).IsRequired().HasMaxLength(100);
            builder.Property(u => u.email).IsRequired().HasMaxLength(255);
            builder.Property(u => u.passwordHash).IsRequired().HasMaxLength(255);
            builder.Property(u => u.phone).HasMaxLength(20);
            builder.Property(u => u.avatarUrl).HasMaxLength(500);
            builder.Property(u => u.roleId).IsRequired();
            builder.Property(u => u.status).IsRequired().HasConversion<string>();

            // Configure navigation properties
            builder.HasOne(u => u.Role)
                   .WithMany(ur => ur.Users)
                   .HasForeignKey(u => u.roleId).OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(u => u.Payments)
                   .WithOne(p => p.User)
                   .HasForeignKey(p => p.userId).OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(u => u.Feedbacks)
                   .WithOne(f => f.User)
                   .HasForeignKey(f => f.userId).OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(u => u.InstructorProfile)
               .WithOne(ip => ip.User)
               .HasForeignKey<InstructorProfile>("userId");

            builder.HasOne(u => u.StudentProfile)
                   .WithOne(sp => sp.User)
                   .HasForeignKey<StudentProfile>("userId");

            builder.HasMany(u => u.Enrollments)
                   .WithOne(e => e.User)
                   .HasForeignKey(e => e.userId).OnDelete(DeleteBehavior.NoAction);



            builder.HasMany(u => u.Conversations)
                   .WithMany(cp => cp.Participants)
                   .UsingEntity(j => j.ToTable("conversation_participant"));

            builder.HasMany(u => u.StudentReports)
                   .WithOne(sr => sr.StudentUser)
                   .HasForeignKey(sr => sr.reporterStudentId).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(u => u.InstructorReports)
                   .WithOne(sr => sr.InstructorUser)
                   .HasForeignKey(sr => sr.reportedInstructorId).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(u => u.Messages)
                   .WithOne(m => m.User)
                   .HasForeignKey(m => m.senderId).OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(u => u.StudentPlans)
                   .WithOne(sp => sp.Student)
                   .HasForeignKey(sp => sp.studentId).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(u => u.StaffCreatePlans)
                   .WithOne(sp => sp.Staff)
                   .HasForeignKey(sp => sp.createdByStaffId).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(u => u.TestAttempts)
                   .WithOne(ta => ta.User)
                   .HasForeignKey(ta => ta.userId).OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(u => u.CreatedTests)
                   .WithOne(t => t.CreatedByUser)
                   .HasForeignKey(t => t.createdByUserId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
