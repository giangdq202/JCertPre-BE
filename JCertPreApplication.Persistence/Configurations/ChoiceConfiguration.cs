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
    public class ChoiceConfiguration : IEntityTypeConfiguration<Choice>
    {
        public void Configure(EntityTypeBuilder<Choice> builder)
        {
            // Configure primary key
            builder.HasKey(c => c.choiceId);

            // Configure required properties
            builder.Property(c => c.questionId).IsRequired();
            builder.Property(c => c.choiceText).IsRequired();
            builder.Property(c => c.isCorrect).IsRequired();

            // Configure foreign key relationship
            builder.HasOne(c => c.Question)
                   .WithMany(q => q.Choices)
                   .HasForeignKey(c => c.questionId).OnDelete(DeleteBehavior.NoAction);

            // Configure navigation property
            builder.HasMany(c => c.AttemptAnswers)
                   .WithOne(aa => aa.Choice)
                   .HasForeignKey(aa => aa.choiceId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
