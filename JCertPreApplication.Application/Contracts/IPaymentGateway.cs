using JCertPreApplication.Application.Dtos.Payment;

namespace JCertPreApplication.Application.Contracts;

public interface IPaymentGateway
{
    Task<CreatePaymentResultDto> CreatePaymentLinkAsync(PaymentDataDto paymentData);
    Task<PaymentLinkInformationDto> GetPaymentLinkInformationAsync(long orderId);
    Task<PaymentLinkInformationDto> CancelPaymentLinkAsync(long orderId, string? cancellationReason = null);
    Task<string> ConfirmWebhookAsync(string webhookUrl);
    WebhookDataDto VerifyPaymentWebhookData(WebhookTypeDto webhookBody);
}
