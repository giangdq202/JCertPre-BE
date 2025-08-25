using JCertPreApplication.Domain.Entities;

namespace JCertPreApplication.UnitTests.Common.Builders;

public class CreditTransactionBuilder
{
    private CreditTransaction _creditTransaction;

    public CreditTransactionBuilder()
    {
        _creditTransaction = new CreditTransaction
        {
            transaction_id = Guid.NewGuid(),
            user_id = Guid.NewGuid(),
            amount = 100,
            balance_before = 200,
            balance_after = 300,
            description = "Test credit transaction",
            created_at = DateTime.UtcNow
        };
    }

    public static CreditTransactionBuilder Create() => new CreditTransactionBuilder();

    public CreditTransactionBuilder WithTransactionId(Guid transactionId)
    {
        _creditTransaction.transaction_id = transactionId;
        return this;
    }

    public CreditTransactionBuilder WithUserId(Guid userId)
    {
        _creditTransaction.user_id = userId;
        return this;
    }

    public CreditTransactionBuilder WithAmount(int amount)
    {
        _creditTransaction.amount = amount;
        return this;
    }

    public CreditTransactionBuilder WithBalanceBefore(int balanceBefore)
    {
        _creditTransaction.balance_before = balanceBefore;
        return this;
    }

    public CreditTransactionBuilder WithBalanceAfter(int balanceAfter)
    {
        _creditTransaction.balance_after = balanceAfter;
        return this;
    }

    public CreditTransactionBuilder WithDescription(string description)
    {
        _creditTransaction.description = description;
        return this;
    }

    public CreditTransactionBuilder WithCreatedAt(DateTime createdAt)
    {
        _creditTransaction.created_at = createdAt;
        return this;
    }

    public CreditTransactionBuilder AsDebit(int amount)
    {
        _creditTransaction.amount = -Math.Abs(amount);
        return this;
    }

    public CreditTransactionBuilder AsCredit(int amount)
    {
        _creditTransaction.amount = Math.Abs(amount);
        return this;
    }

    public CreditTransaction Build() => _creditTransaction;
}
