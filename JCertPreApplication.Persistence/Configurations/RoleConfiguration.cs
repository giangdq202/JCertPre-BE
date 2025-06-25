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
            builder.HasKey(r => r.roleId);

            // Configure required properties and constraints
            builder.Property(r => r.roleName).IsRequired().HasMaxLength(100);
            builder.Property(r => r.description).HasMaxLength(500);

            // Configure navigation property
            builder.HasMany(r => r.UserRoles)
                   .WithOne(ur => ur.Role)
                   .HasForeignKey(ur => ur.roleId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
