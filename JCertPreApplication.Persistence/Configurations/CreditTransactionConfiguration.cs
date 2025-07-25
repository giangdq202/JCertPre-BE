using JCertPreApplication.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JCertPreApplication.Persistence.Configurations
{
    public class CreditTransactionConfiguration : IEntityTypeConfiguration<CreditTransaction>
    {
        public void Configure(EntityTypeBuilder<CreditTransaction> builder)
        {
            builder.ToTable("credit_transactions");
            builder.HasKey(ct => ct.transaction_id);

            builder.Property(ct => ct.user_id)
                .IsRequired();

            builder.Property(ct => ct.amount)
                .IsRequired();

            builder.Property(ct => ct.balance_before)
                .IsRequired();

            builder.Property(ct => ct.balance_after)
                .IsRequired();

            builder.Property(ct => ct.description)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(ct => ct.created_at)
                .IsRequired();

            builder.HasOne(ct => ct.User)
                .WithMany(u => u.CreditTransactions)
                .HasForeignKey(ct => ct.user_id)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
