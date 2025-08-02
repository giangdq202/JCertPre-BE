namespace JCertPreApplication.Application.Contracts;

public interface IMailService
{
    Task SendEmailAsync(string to, string subject, string body, bool isHtml = false);
    Task SendEmailAsync(string to, string subject, string body, IEnumerable<string>? cc = null, IEnumerable<string>? bcc = null, bool isHtml = false);
    Task SendEmailWithAttachmentAsync(string to, string subject, string body, IEnumerable<EmailAttachment> attachments, bool isHtml = false);
    Task SendBulkEmailAsync(IEnumerable<string> recipients, string subject, string body, bool isHtml = false);
    Task SendTemplateEmailAsync(string to, string templateName, object templateData);
}

public class EmailAttachment
{
    public string FileName { get; set; } = string.Empty;
    public byte[] Content { get; set; } = Array.Empty<byte>();
    public string ContentType { get; set; } = string.Empty;
}
