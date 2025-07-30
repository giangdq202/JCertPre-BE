using JCertPreApplication.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JCertPreApplication.Persistence.Configurations
{
    public class LivestreamConfiguration : IEntityTypeConfiguration<Livestream>
    {
        public void Configure(EntityTypeBuilder<Livestream> builder)
        {
            builder.ToTable("livestream");
            builder.HasKey(ls => ls.livestreamId);

            // Configure properties
            builder.Property(ls => ls.courseId).IsRequired();
            builder.Property(ls => ls.description).HasMaxLength(500);
            builder.Property(ls => ls.scheduledDateTime).IsRequired();
            builder.Property(ls => ls.durationMinutes).IsRequired();
            builder.Property(ls => ls.status).IsRequired().HasConversion<string>();

            // Configure relationships
            builder.HasOne(ls => ls.Course)
                .WithMany(c => c.Livestreams)
                .HasForeignKey(ls => ls.courseId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
