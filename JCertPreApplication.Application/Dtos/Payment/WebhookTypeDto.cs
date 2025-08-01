namespace JCertPreApplication.Application.Dtos.Payment;

public class WebhookTypeDto
{
    public string Code { get; set; } = string.Empty;
    public string Desc { get; set; } = string.Empty;
    public WebhookDataDto Data { get; set; } = new();
    public string Signature { get; set; } = string.Empty;
    public bool Success { get; set; }
}
