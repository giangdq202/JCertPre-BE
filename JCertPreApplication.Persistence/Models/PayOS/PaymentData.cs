namespace JCertPreApplication.Persistence.Models.PayOS;

public class PaymentData
{
    public long OrderCode { get; set; }
    public int Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public List<ItemData> Items { get; set; } = new();
    public string ReturnUrl { get; set; } = string.Empty;
    public string CancelUrl { get; set; } = string.Empty;
}
