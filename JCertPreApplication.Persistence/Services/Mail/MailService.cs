
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using JCertPreApplication.Domain.Configuration;
using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.Mail;

namespace JCertPreApplication.Persistence.Services.Mail;

public class MailService : IMailService
{
    private readonly SmtpConfiguration _smtpConfiguration;
    private readonly ILogger<MailService> _logger;
    private readonly string _templatePath;

    public MailService(IOptions<SmtpConfiguration> smtpConfiguration, ILogger<MailService> logger)
    {
        _smtpConfiguration = smtpConfiguration.Value;
        _logger = logger;
        _templatePath = Path.Combine(AppContext.BaseDirectory, "Templates");
    }

    public async Task<EmailTemplate> LoadEmailTemplate(string templateName)
    {
        try
        {
            var htmlFilePath = Path.Combine(_templatePath, $"{templateName}-email.html");
            if (System.IO.File.Exists(htmlFilePath))
            {
                var htmlContent = await System.IO.File.ReadAllTextAsync(htmlFilePath);
                var template = new EmailTemplate
                {
                    Name = templateName,
                    HtmlBody = htmlContent,
                    TextBody = ExtractTextFromHtml(htmlContent),
                    Subject = templateName switch
                    {
                        "welcome" => "🎉 Welcome to JCert Platform, {{Name}}!",
                        "password-reset" => "🔐 Reset Your JCert Password",
                        "password-changed" => "✅ Your JCert Password Has Been Changed",
                        _ => "JCert Platform Notification"
                    }
                };
                return template;
            }

            var templates = new Dictionary<string, EmailTemplate>
            {
                ["welcome"] = new EmailTemplate
                {
                    Name = "welcome",
                    Subject = "Welcome to JCert Application, {{Name}}!",
                    HtmlBody = "<h1>Welcome {{Name}}!</h1><p>Thank you for joining our platform.</p><p>Your email: {{Email}}</p>",
                    TextBody = "Welcome {{Name}}! Thank you for joining our platform. Your email: {{Email}}"
                },
                ["password-reset"] = new EmailTemplate
                {
                    Name = "password-reset",
                    Subject = "🔐 Đặt lại mật khẩu JCertPre",
                    HtmlBody = "...",
                    TextBody = "Hi {{Name}}, click this link to reset your JCert password: {{ResetLink}} (expires in {{ExpiryMinutes}} minutes). If you didn't request this, please ignore this email."
                },
                ["password-changed"] = new EmailTemplate
                {
                    Name = "password-changed",
                    Subject = "✅ Mật khẩu JCertPre đã được thay đổi",
                    HtmlBody = "...",
                    TextBody = "Hi {{Name}}, your password has been changed at {{ChangeTime}} from IP {{IpAddress}}. Contact support if this wasn't you."
                }
            };

            if (templates.TryGetValue(templateName.ToLower(), out var fallbackTemplate))
            {
                return fallbackTemplate;
            }

            throw new System.ArgumentException($"Email template '{templateName}' not found");
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "Failed to load email template '{TemplateName}'", templateName);
            throw;
        }
    }

    // Implement IMailService methods
    public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = false)
    {
        await SendEmailAsync(to, subject, body, null, null, isHtml);
    }

    public async Task SendEmailAsync(string to, string subject, string body, IEnumerable<string>? cc = null, IEnumerable<string>? bcc = null, bool isHtml = false)
    {
        try
        {
            using var client = new SmtpClient(_smtpConfiguration.Host, _smtpConfiguration.Port)
            {
                Credentials = new NetworkCredential(_smtpConfiguration.Username, _smtpConfiguration.Password),
                EnableSsl = _smtpConfiguration.EnableSsl
            };

            using var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpConfiguration.FromEmail, _smtpConfiguration.FromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };

            mailMessage.To.Add(to);

            if (cc != null)
            {
                foreach (var ccEmail in cc)
                {
                    mailMessage.CC.Add(ccEmail);
                }
            }

            if (bcc != null)
            {
                foreach (var bccEmail in bcc)
                {
                    mailMessage.Bcc.Add(bccEmail);
                }
            }

            await client.SendMailAsync(mailMessage);
            _logger.LogInformation("Email sent successfully to {To}", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}", to);
            throw;
        }
    }

    public async Task SendEmailWithAttachmentAsync(string to, string subject, string body, IEnumerable<JCertPreApplication.Application.Contracts.EmailAttachment> attachments, bool isHtml = false)
    {
        try
        {
            using var client = new SmtpClient(_smtpConfiguration.Host, _smtpConfiguration.Port)
            {
                Credentials = new NetworkCredential(_smtpConfiguration.Username, _smtpConfiguration.Password),
                EnableSsl = _smtpConfiguration.EnableSsl
            };

            using var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpConfiguration.FromEmail, _smtpConfiguration.FromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };

            mailMessage.To.Add(to);

            foreach (var attachment in attachments)
            {
                var stream = new MemoryStream(attachment.Content);
                var mailAttachment = new Attachment(stream, attachment.FileName, attachment.ContentType);
                mailMessage.Attachments.Add(mailAttachment);
            }

            await client.SendMailAsync(mailMessage);
            _logger.LogInformation("Email with attachments sent successfully to {To}", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email with attachments to {To}", to);
            throw;
        }
    }

    public async Task SendBulkEmailAsync(IEnumerable<string> recipients, string subject, string body, bool isHtml = false)
    {
        var tasks = recipients.Select(recipient => SendEmailAsync(recipient, subject, body, isHtml));
        await Task.WhenAll(tasks);
    }

    public async Task SendTemplateEmailAsync(string to, string templateName, object templateData)
    {
        try
        {
            var template = await LoadEmailTemplate(templateName);
            var renderedSubject = RenderTemplate(template.Subject, templateData);
            var renderedBody = RenderTemplate(template.HtmlBody, templateData);

            await SendEmailAsync(to, renderedSubject, renderedBody, isHtml: true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send template email '{TemplateName}' to {To}", templateName, to);
            throw;
        }
    }

    private static string ExtractTextFromHtml(string html)
    {
        var text = Regex.Replace(html, "<.*?>", string.Empty);
        text = Regex.Replace(text, "\\s+", " ").Trim();
        return text;
    }

    public static string RenderTemplate(string template, object data)
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
