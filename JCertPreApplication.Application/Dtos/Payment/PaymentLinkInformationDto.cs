namespace JCertPreApplication.Application.Dtos.Payment;

public class PaymentLinkInformationDto
{
    public string Id { get; set; } = string.Empty;
    public long OrderCode { get; set; }
    public int Amount { get; set; }
    public int AmountPaid { get; set; }
    public int AmountRemaining { get; set; }
    public string Status { get; set; } = string.Empty;
    public string CreatedAt { get; set; } = string.Empty;
    public List<TransactionDto> Transactions { get; set; } = new();
    public string? CancellationReason { get; set; }
    public string? CanceledAt { get; set; }
}
