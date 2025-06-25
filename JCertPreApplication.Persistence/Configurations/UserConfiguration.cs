using JCertPreApplication.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCertPreApplication.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // Configure primary key
            builder.HasKey(u => u.userId);

            // Configure required properties and constraints
            builder.Property(u => u.fullName).IsRequired().HasMaxLength(100);
            builder.Property(u => u.email).IsRequired().HasMaxLength(255);
            builder.Property(u => u.passwordHash).IsRequired().HasMaxLength(255);
            builder.Property(u => u.phone).HasMaxLength(20);
            builder.Property(u => u.avatarUrl).HasMaxLength(500);

            // Configure navigation properties
            builder.HasMany(u => u.UserRoles)
                   .WithOne(ur => ur.User)
                   .HasForeignKey(u => u.userId);

            builder.HasMany(u => u.Payments)
                   .WithOne(p => p.User)
                   .HasForeignKey(p => p.userId);

            builder.HasMany(u => u.Feedbacks)
                   .WithOne(f => f.User)
                   .HasForeignKey(f => f.userId);

            builder.HasOne(u => u.InstructorProfile)
               .WithOne(ip => ip.User)
               .HasForeignKey<InstructorProfile>("userId");

            builder.HasOne(u => u.StudentProfile)
                   .WithOne(sp => sp.User)
                   .HasForeignKey<StudentProfile>("userId");

            builder.HasMany(u => u.Enrollments)
                   .WithOne(e => e.User)
                   .HasForeignKey(e => e.userId);

            builder.HasMany(u => u.Courses)
                   .WithOne(c => c.User)
                   .HasForeignKey(c => c.staffCreateUserId);

            builder.HasMany(u => u.ConversationParticipants)
                   .WithOne(cp => cp.User)
                   .HasForeignKey(u => u.userId);

            builder.HasMany(u => u.StudentReports)
                   .WithOne(sr => sr.StudentUser)
                   .HasForeignKey(u => u.StudentUser);

            builder.HasMany(u => u.Messages)
                   .WithOne(m => m.User)
                   .HasForeignKey(u => u.senderId);

            builder.HasMany(u => u.StudentPlans)
                   .WithOne(sp => sp.Student)
                   .HasForeignKey(u => u.studentId);

            builder.HasMany(u => u.TestAttempts)
                   .WithOne(ta => ta.User)
                   .HasForeignKey(u => u.userId);

            builder.HasMany(u => u.CreatedTests)
                   .WithOne(t => t.CreatedByUser)
                   .HasForeignKey(t => t.createdByUserId);
        }
    }
}
