using JCertPreApplication.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace JCertPreApplication.Persistence.Configurations
{
    public class LivestreamConfiguration : IEntityTypeConfiguration<Livestream>
    {
        public void Configure(EntityTypeBuilder<Livestream> builder)
        {
            // Configure primary key
            builder.ToTable("livestream");
            builder.HasKey(ls => ls.livestreamId);

            // Configure required properties
            builder.Property(ls => ls.courseId).IsRequired();
            builder.Property(ls => ls.title).IsRequired();
            builder.Property(ls => ls.startTime).IsRequired();
            builder.Property(ls => ls.endTime).IsRequired();
            builder.Property(ls => ls.meetingUrl).IsRequired();
            builder.Property(ls => ls.recordingUrl).IsRequired();

            // Configure foreign key relationship
            builder.HasOne(ls => ls.Course)
                   .WithMany(c => c.Livestreams)
                   .HasForeignKey(ls => ls.courseId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
