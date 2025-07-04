using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using JCertPreApplication.Domain.Entities;

namespace JCertPreApplication.Persistence.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            // Configure primary key
            builder.ToTable("role");
            builder.HasKey(r => r.roleId);

            // Configure required properties and constraints
            builder.Property(r => r.roleName).IsRequired().HasMaxLength(100);
            builder.Property(r => r.description).HasMaxLength(500);

            // Configure navigation property
            builder.HasMany(r => r.Users)
                   .WithOne(ur => ur.Role)
                   .HasForeignKey(ur => ur.roleId).OnDelete(DeleteBehavior.NoAction);

            builder.HasData(
                new Role
                {
                    roleId = Guid.Parse("8dd36044-84d4-4e4b-8162-34b7a421657c"),
                    roleName = "STUDENT",
                    description = "Student role"
                },
                new Role
                {
                    roleId = Guid.Parse("8174528c-7f5b-4277-aa1a-1150e7b8b275"),
                    roleName = "INSTRUCTOR",
                    description = "Instructor role"
                },
                new Role
                {
                    roleId = Guid.Parse("0d1c9d64-3be8-4d5c-9ad0-062f83a3a7f8"),
                    roleName = "ACADEMIC_MANAGER",
                    description = "Academic Manager role"
                },
                new Role
                {
                    roleId = Guid.Parse("d500140c-99c5-452f-b44c-a3b4e650d0e6"),
                    roleName = "ADMIN",
                    description = "Administrator role"
                }
            );
        }
    }
}
