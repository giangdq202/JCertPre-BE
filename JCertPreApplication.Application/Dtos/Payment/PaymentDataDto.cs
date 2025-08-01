namespace JCertPreApplication.Application.Dtos.Payment;

public class PaymentDataDto
{
    public long OrderCode { get; set; }
    public int Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public List<ItemDataDto> Items { get; set; } = new();
    public string ReturnUrl { get; set; } = string.Empty;
    public string CancelUrl { get; set; } = string.Empty;
}
