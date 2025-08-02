
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using JCertPreApplication.Domain.Configuration;

namespace JCertPreApplication.Persistence.Services.Mail;

public class MailService
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
            if (File.Exists(htmlFilePath))
            {
                var htmlContent = await File.ReadAllTextAsync(htmlFilePath);
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

public class EmailTemplate
{
    public string Name { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string HtmlBody { get; set; } = string.Empty;
    public string TextBody { get; set; } = string.Empty;
}
