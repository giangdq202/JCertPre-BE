namespace JCertPreApplication.Persistence.Models.PayOS;

public class WebhookType
{
    public string Code { get; set; } = string.Empty;
    public string Desc { get; set; } = string.Empty;
    public WebhookData Data { get; set; } = new();
    public string Signature { get; set; } = string.Empty;
    public bool Success { get; set; }
}
