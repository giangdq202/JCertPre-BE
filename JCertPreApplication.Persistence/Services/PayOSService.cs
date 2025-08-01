using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.Payment;
using JCertPreApplication.Application.Exceptions;
using Net.payOS;
using Net.payOS.Types;

namespace JCertPreApplication.Persistence.Services;

public class PayOSService : IPaymentGateway
{
    private readonly PayOS _payOS;
    private readonly string _returnUrl;
    private readonly string _cancelUrl;

    public PayOSService(string clientId, string apiKey, string checksumKey, string returnUrl, string cancelUrl)
    {
        _payOS = new PayOS(clientId, apiKey, checksumKey);
        _returnUrl = returnUrl;
        _cancelUrl = cancelUrl;
    }

    public async Task<CreatePaymentResultDto> CreatePaymentLinkAsync(PaymentDataDto paymentData)
    {
        try
        {
            var paymentDataRequest = new Net.payOS.Types.PaymentData(
                orderCode: paymentData.OrderCode,
                amount: paymentData.Amount,
                description: paymentData.Description,
                items: paymentData.Items.Select(i => new Net.payOS.Types.ItemData(i.Name, i.Quantity, i.Price)).ToList(),
                returnUrl: _returnUrl,
                cancelUrl: _cancelUrl
            );

            var createPaymentResult = await _payOS.createPaymentLink(paymentDataRequest);

            return new CreatePaymentResultDto
            {
                CheckoutUrl = createPaymentResult.checkoutUrl,
                QrCode = createPaymentResult.qrCode,
                PaymentLinkId = createPaymentResult.paymentLinkId,
                OrderCode = createPaymentResult.orderCode,
                Amount = createPaymentResult.amount,
                Description = createPaymentResult.description,
                Status = createPaymentResult.status
            };
        }
        catch (Exception ex)
        {
            throw ApiException.InternalServerError("PAYOS_CREATE_PAYMENT_ERROR", $"Error creating payment link: {ex.Message}");
        }
    }

    public async Task<PaymentLinkInformationDto> GetPaymentLinkInformationAsync(long orderId)
    {
        try
        {
            var paymentLinkInfo = await _payOS.getPaymentLinkInformation(orderId);

            return new PaymentLinkInformationDto
            {
                Id = paymentLinkInfo.id,
                OrderCode = paymentLinkInfo.orderCode,
                Amount = paymentLinkInfo.amount,
                AmountPaid = paymentLinkInfo.amountPaid,
                AmountRemaining = paymentLinkInfo.amountRemaining,
                Status = paymentLinkInfo.status,
                CreatedAt = paymentLinkInfo.createdAt,
                Transactions = paymentLinkInfo.transactions?.Select(t => new TransactionDto
                {
                    Reference = t.reference,
                    Amount = t.amount,
                    AccountNumber = t.accountNumber,
                    Description = t.description,
                    TransactionDateTime = t.transactionDateTime,
                    VirtualAccountName = t.virtualAccountName ?? string.Empty,
                    VirtualAccountNumber = t.virtualAccountNumber ?? string.Empty,
                    CounterAccountBankId = t.counterAccountBankId ?? string.Empty,
                    CounterAccountBankName = t.counterAccountBankName ?? string.Empty,
                    CounterAccountName = t.counterAccountName ?? string.Empty,
                    CounterAccountNumber = t.counterAccountNumber ?? string.Empty
                }).ToList() ?? new List<TransactionDto>(),
                CancellationReason = paymentLinkInfo.cancellationReason,
                CanceledAt = paymentLinkInfo.canceledAt
            };
        }
        catch (Exception ex)
        {
            throw ApiException.InternalServerError("PAYOS_GET_PAYMENT_INFO_ERROR", $"Error getting payment link information: {ex.Message}");
        }
    }

    public async Task<PaymentLinkInformationDto> CancelPaymentLinkAsync(long orderId, string? cancellationReason = null)
    {
        try
        {
            var cancelledPaymentLink = await _payOS.cancelPaymentLink(orderId, cancellationReason);

            return new PaymentLinkInformationDto
            {
                Id = cancelledPaymentLink.id,
                OrderCode = cancelledPaymentLink.orderCode,
                Amount = cancelledPaymentLink.amount,
                AmountPaid = cancelledPaymentLink.amountPaid,
                AmountRemaining = cancelledPaymentLink.amountRemaining,
                Status = cancelledPaymentLink.status,
                CreatedAt = cancelledPaymentLink.createdAt,
                Transactions = cancelledPaymentLink.transactions?.Select(t => new TransactionDto
                {
                    Reference = t.reference,
                    Amount = t.amount,
                    AccountNumber = t.accountNumber,
                    Description = t.description,
                    TransactionDateTime = t.transactionDateTime,
                    VirtualAccountName = t.virtualAccountName ?? string.Empty,
                    VirtualAccountNumber = t.virtualAccountNumber ?? string.Empty,
                    CounterAccountBankId = t.counterAccountBankId ?? string.Empty,
                    CounterAccountBankName = t.counterAccountBankName ?? string.Empty,
                    CounterAccountName = t.counterAccountName ?? string.Empty,
                    CounterAccountNumber = t.counterAccountNumber ?? string.Empty
                }).ToList() ?? new List<TransactionDto>(),
                CancellationReason = cancelledPaymentLink.cancellationReason,
                CanceledAt = cancelledPaymentLink.canceledAt
            };
        }
        catch (Exception ex)
        {
            throw ApiException.InternalServerError("PAYOS_CANCEL_PAYMENT_ERROR", $"Error cancelling payment link: {ex.Message}");
        }
    }

    public async Task<string> ConfirmWebhookAsync(string webhookUrl)
    {
        try
        {
            var confirmationResult = await _payOS.confirmWebhook(webhookUrl);
            return confirmationResult;
        }
        catch (Exception ex)
        {
            throw ApiException.InternalServerError("PAYOS_CONFIRM_WEBHOOK_ERROR", $"Error confirming webhook: {ex.Message}");
        }
    }

    public WebhookDataDto VerifyPaymentWebhookData(WebhookTypeDto webhookBody)
    {
        try
        {
            var webhookData = new Net.payOS.Types.WebhookData(
                orderCode: webhookBody.Data.OrderCode,
                amount: webhookBody.Data.Amount,
                description: webhookBody.Data.Description,
                accountNumber: webhookBody.Data.AccountNumber,
                reference: webhookBody.Data.Reference,
                transactionDateTime: webhookBody.Data.TransactionDateTime,
                currency: webhookBody.Data.Currency,
                paymentLinkId: webhookBody.Data.PaymentLinkId,
                code: webhookBody.Data.Code,
                desc: webhookBody.Data.Desc,
                counterAccountBankId: webhookBody.Data.CounterAccountBankId,
                counterAccountBankName: webhookBody.Data.CounterAccountBankName,
                counterAccountName: webhookBody.Data.CounterAccountName,
                counterAccountNumber: webhookBody.Data.CounterAccountNumber,
                virtualAccountName: webhookBody.Data.VirtualAccountName,
                virtualAccountNumber: webhookBody.Data.VirtualAccountNumber
            );

            var webhookType = new Net.payOS.Types.WebhookType(
                code: webhookBody.Code,
                desc: webhookBody.Desc,
                success: webhookBody.Success,
                data: webhookData,
                signature: webhookBody.Signature
            );

            var verifiedData = _payOS.verifyPaymentWebhookData(webhookType);

            return new WebhookDataDto
            {
                OrderCode = verifiedData.orderCode,
                Amount = verifiedData.amount,
                Description = verifiedData.description,
                AccountNumber = verifiedData.accountNumber,
                Reference = verifiedData.reference,
                TransactionDateTime = verifiedData.transactionDateTime,
                Currency = verifiedData.currency,
                PaymentLinkId = verifiedData.paymentLinkId,
                Code = verifiedData.code,
                Desc = verifiedData.desc,
                CounterAccountBankId = verifiedData.counterAccountBankId ?? string.Empty,
                CounterAccountBankName = verifiedData.counterAccountBankName ?? string.Empty,
                CounterAccountName = verifiedData.counterAccountName ?? string.Empty,
                CounterAccountNumber = verifiedData.counterAccountNumber ?? string.Empty,
                VirtualAccountName = verifiedData.virtualAccountName ?? string.Empty,
                VirtualAccountNumber = verifiedData.virtualAccountNumber ?? string.Empty
            };
        }
        catch (Exception ex)
        {
            throw ApiException.InternalServerError("PAYOS_VERIFY_WEBHOOK_ERROR", $"Error verifying payment webhook data: {ex.Message}");
        }
    }
}
