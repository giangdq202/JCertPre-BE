namespace JCertPreApplication.Application.Dtos.Payment
{
    public class CreateCreditPurchaseRequestDto
    {
        public Guid UserId { get; set; }
        public int CreditAmount { get; set; }  // Số credit muốn mua (rate 1:1)
    }

    public class CreateCreditPurchaseResponseDto
    {
        public string PaymentUrl { get; set; } = string.Empty;
        public long OrderCode { get; set; }
        public int Amount { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    public class ConfirmWebhookRequestDto
    {
        public string WebhookUrl { get; set; } = string.Empty;
    }
}
