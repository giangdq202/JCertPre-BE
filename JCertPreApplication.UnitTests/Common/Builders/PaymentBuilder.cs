using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.UnitTests.Common.Builders;

public class PaymentBuilder
{
    private Payment _payment;

    public PaymentBuilder()
    {
        _payment = new Payment
        {
            paymentId = Guid.NewGuid(),
            userId = Guid.NewGuid(),
            amount = 100.00m,
            PaymentType = PaymentType.Credit,
            transactionId = $"PAY_{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid().ToString("N")[..8]}",
            status = PaymentStatus.Completed,
            createdAt = DateTime.UtcNow,
            description = "Test payment"
        };
    }

    public static PaymentBuilder Create() => new PaymentBuilder();

    public PaymentBuilder WithId(Guid paymentId)
    {
        _payment.paymentId = paymentId;
        return this;
    }

    public PaymentBuilder WithUserId(Guid userId)
    {
        _payment.userId = userId;
        return this;
    }

    public PaymentBuilder WithAmount(decimal amount)
    {
        _payment.amount = amount;
        return this;
    }

    public PaymentBuilder WithPaymentType(PaymentType paymentType)
    {
        _payment.PaymentType = paymentType;
        return this;
    }

    public PaymentBuilder WithTransactionId(string transactionId)
    {
        _payment.transactionId = transactionId;
        return this;
    }

    public PaymentBuilder WithStatus(PaymentStatus status)
    {
        _payment.status = status;
        return this;
    }

    public PaymentBuilder WithCreatedAt(DateTime createdAt)
    {
        _payment.createdAt = createdAt;
        return this;
    }

    public PaymentBuilder WithDescription(string description)
    {
        _payment.description = description;
        return this;
    }

    public PaymentBuilder AsPending()
    {
        _payment.status = PaymentStatus.Pending;
        return this;
    }

    public PaymentBuilder AsCompleted()
    {
        _payment.status = PaymentStatus.Completed;
        return this;
    }

    public PaymentBuilder AsFailed()
    {
        _payment.status = PaymentStatus.Failed;
        return this;
    }

    public Payment Build() => _payment;
}
