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
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            // Configure primary key
            builder.HasKey(p => p.paymentId);

            // Configure required properties and constraints
            builder.Property(p => p.userId).IsRequired();
            builder.Property(p => p.amount).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(p => p.currency).IsRequired().HasMaxLength(50);
            builder.Property(p => p.paymentMethod).IsRequired().HasMaxLength(50);
            builder.Property(p => p.transactionId).IsRequired().HasMaxLength(100);
            builder.Property(p => p.status).IsRequired();
            builder.Property(p => p.createdAt).IsRequired();
            builder.Property(p => p.description).HasMaxLength(255);

            // Configure foreign key relationship
            builder.HasOne(p => p.User)
                   .WithMany()
                   .HasForeignKey(p => p.userId);
        }
    }
}
