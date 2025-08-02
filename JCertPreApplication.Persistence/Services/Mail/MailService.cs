using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Domain.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JCertPreApplication.Persistence.Services.Mail;

public class MailService : IMailService
{
    private readonly SmtpConfiguration _smtpConfiguration;
    private readonly ILogger<MailService> _logger;

    public MailService(IOptions<SmtpConfiguration> smtpConfiguration, ILogger<MailService> logger)
    {
        _smtpConfiguration = smtpConfiguration.Value;
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = false)
    {
        try
        {
            ValidateEmailAddress(to);
            
            using var client = CreateSmtpClient();
            using var message = CreateMailMessage(to, subject, body, isHtml);
            
            await client.SendMailAsync(message);
            _logger.LogInformation("Email sent successfully to {To} with subject: {Subject}", to, subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To} with subject: {Subject}", to, subject);
            throw;
        }
    }

    public async Task SendEmailAsync(string to, string subject, string body, IEnumerable<string>? cc = null, IEnumerable<string>? bcc = null, bool isHtml = false)
    {
        try
        {
            ValidateEmailAddress(to);
            
            using var client = CreateSmtpClient();
            using var message = CreateMailMessage(to, subject, body, isHtml);

            if (cc != null)
            {
                foreach (var ccEmail in cc)
                {
                    ValidateEmailAddress(ccEmail);
                    message.CC.Add(ccEmail);
                }
            }

            if (bcc != null)
            {
                foreach (var bccEmail in bcc)
                {
                    ValidateEmailAddress(bccEmail);
                    message.Bcc.Add(bccEmail);
                }
            }
            
            await client.SendMailAsync(message);
            _logger.LogInformation("Email sent successfully to {To} with CC: {CC}, BCC: {BCC}", to, 
                cc != null ? string.Join(",", cc) : "None", 
                bcc != null ? string.Join(",", bcc) : "None");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To} with subject: {Subject}", to, subject);
            throw;
        }
    }

    public async Task SendEmailWithAttachmentAsync(string to, string subject, string body, IEnumerable<EmailAttachment> attachments, bool isHtml = false)
    {
        try
        {
            ValidateEmailAddress(to);
            
            using var client = CreateSmtpClient();
            using var message = CreateMailMessage(to, subject, body, isHtml);

            foreach (var attachment in attachments)
            {
                var memoryStream = new MemoryStream(attachment.Content);
                var mailAttachment = new Attachment(memoryStream, attachment.FileName, attachment.ContentType);
                message.Attachments.Add(mailAttachment);
            }
            
            await client.SendMailAsync(message);
            _logger.LogInformation("Email with {AttachmentCount} attachments sent successfully to {To}", 
                attachments.Count(), to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email with attachments to {To}", to);
            throw;
        }
    }

    public async Task SendBulkEmailAsync(IEnumerable<string> recipients, string subject, string body, bool isHtml = false)
    {
        var tasks = new List<Task>();
        
        foreach (var recipient in recipients)
        {
            tasks.Add(SendEmailAsync(recipient, subject, body, isHtml));
        }

        try
        {
            await Task.WhenAll(tasks);
            _logger.LogInformation("Bulk email sent successfully to {RecipientCount} recipients", recipients.Count());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send bulk email to some recipients");
            throw;
        }
    }

    public async Task SendTemplateEmailAsync(string to, string templateName, object templateData)
    {
        try
        {
            var template = await LoadEmailTemplate(templateName);
            var renderedBody = RenderTemplate(template.HtmlBody, templateData);
            var renderedSubject = RenderTemplate(template.Subject, templateData);

            await SendEmailAsync(to, renderedSubject, renderedBody, isHtml: true);
            _logger.LogInformation("Template email '{TemplateName}' sent successfully to {To}", templateName, to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send template email '{TemplateName}' to {To}", templateName, to);
            throw;
        }
    }

    private SmtpClient CreateSmtpClient()
    {
        var client = new SmtpClient(_smtpConfiguration.Host, _smtpConfiguration.Port)
        {
            Credentials = new NetworkCredential(_smtpConfiguration.Username, _smtpConfiguration.Password),
            EnableSsl = _smtpConfiguration.EnableSsl,
            Timeout = _smtpConfiguration.Timeout
        };

        return client;
    }

    private MailMessage CreateMailMessage(string to, string subject, string body, bool isHtml)
    {
        var message = new MailMessage
        {
            From = new MailAddress(_smtpConfiguration.FromEmail, _smtpConfiguration.FromName),
            Subject = subject,
            Body = body,
            IsBodyHtml = isHtml,
            BodyEncoding = Encoding.UTF8,
            SubjectEncoding = Encoding.UTF8
        };

        message.To.Add(to);
        return message;
    }

    private static void ValidateEmailAddress(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email address cannot be null or empty");

        var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        if (!emailRegex.IsMatch(email))
            throw new ArgumentException($"Invalid email address format: {email}");
    }

    private async Task<EmailTemplate> LoadEmailTemplate(string templateName)
    {
        // This is a simple implementation. In a real application, you might load templates from:
        // - Database
        // - File system
        // - External service
        
        var templates = new Dictionary<string, EmailTemplate>
        {
            ["welcome"] = new()
            {
                Name = "welcome",
                Subject = "Welcome to JCert Application, {{Name}}!",
                HtmlBody = "<h1>Welcome {{Name}}!</h1><p>Thank you for joining our platform.</p>",
                TextBody = "Welcome {{Name}}! Thank you for joining our platform."
            },
            ["password-reset"] = new()
            {
                Name = "password-reset",
                Subject = "Reset Your Password",
                HtmlBody = "<h1>Password Reset</h1><p>Click <a href='{{ResetLink}}'>here</a> to reset your password.</p>",
                TextBody = "Password Reset: {{ResetLink}}"
            }
        };

        if (templates.TryGetValue(templateName.ToLower(), out var template))
        {
            return await Task.FromResult(template);
        }

        throw new ArgumentException($"Email template '{templateName}' not found");
    }

    private static string RenderTemplate(string template, object data)
    {
        var result = template;
        var properties = data.GetType().GetProperties();

        foreach (var property in properties)
        {
            var value = property.GetValue(data)?.ToString() ?? string.Empty;
            result = result.Replace($"{{{{{property.Name}}}}}", value);
        }

        return result;
    }
}

public class EmailTemplate
{
    public string Name { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string HtmlBody { get; set; } = string.Empty;
    public string TextBody { get; set; } = string.Empty;
}
