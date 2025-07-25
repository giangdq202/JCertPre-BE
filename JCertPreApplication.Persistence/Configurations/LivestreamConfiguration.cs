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

            builder.Property(ls => ls.lessonId).IsRequired();
            builder.Property(ls => ls.startTime).IsRequired();
            builder.Property(ls => ls.endTime).IsRequired();

            builder.HasOne(ls => ls.Lesson)
                .WithOne(l => l.Livestream)
                .HasForeignKey<Livestream>(ls => ls.lessonId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
