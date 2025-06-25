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
    public class TestAttemptConfiguration : IEntityTypeConfiguration<TestAttempt>
    {
        public void Configure(EntityTypeBuilder<TestAttempt> builder)
        {
            // Configure primary key
            builder.HasKey(ta => ta.attemptId);

            // Configure required properties
            builder.Property(ta => ta.userId).IsRequired();
            builder.Property(ta => ta.testId).IsRequired();
            builder.Property(ta => ta.startTime).IsRequired();
            builder.Property(ta => ta.endTime).IsRequired();
            builder.Property(ta => ta.totalScore).IsRequired();
            builder.Property(ta => ta.languageKnowledgeScore).IsRequired();
            builder.Property(ta => ta.readingScore).IsRequired();
            builder.Property(ta => ta.listeningScore).IsRequired();
            builder.Property(ta => ta.isPass).IsRequired();

            // Configure navigation properties
            builder.HasOne(ta => ta.User)
                   .WithMany(t => t.TestAttempts)
                   .HasForeignKey(ta => ta.userId).OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(ta => ta.Test)
                   .WithMany(t => t.TestAttempts)
                   .HasForeignKey(ta => ta.testId).OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(ta => ta.AttemptAnswers)
                   .WithOne(t => t.TestAttempt)
                   .HasForeignKey(t => t.attemptId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}