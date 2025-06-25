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
    public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            // Định nghĩa khóa composite
            builder.HasKey(ur => new { ur.userId, ur.roleId });

            // Cấu hình thuộc tính
            builder.Property(ur => ur.assignedAt)
                   .IsRequired();

            // Cấu hình mối quan hệ với User
            builder.HasOne(ur => ur.User)
                   .WithMany(u => u.UserRoles) // Có thể WithMany(u => u.UserRoles) nếu User có collection
                   .HasForeignKey(ur => ur.userId)
                   .OnDelete(DeleteBehavior.Restrict); // Điều chỉnh OnDelete tùy ý

            // Cấu hình mối quan hệ với Role
            builder.HasOne(ur => ur.Role)
                   .WithMany(r => r.UserRoles) // Có thể WithMany(r => r.UserRoles) nếu Role có collection
                   .HasForeignKey(ur => ur.roleId)
                   .OnDelete(DeleteBehavior.Restrict); // Điều chỉnh OnDelete tùy ý
        }
    }
}
