namespace JCertPreApplication.Application.Dtos.Mail;

public class EmailTemplate
{
    public string Name { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string HtmlBody { get; set; } = string.Empty;
    public string TextBody { get; set; } = string.Empty;
    public Dictionary<string, object> Data { get; set; } = new();
}
