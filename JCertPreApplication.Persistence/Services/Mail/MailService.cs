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
    private readonly string _templatePath;

    public MailService(IOptions<SmtpConfiguration> smtpConfiguration, ILogger<MailService> logger)
    {
        _smtpConfiguration = smtpConfiguration.Value;
        _logger = logger;
        _templatePath = Path.Combine(AppContext.BaseDirectory, "Templates");
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
        try
        {
            // Try to load from HTML file first
            var htmlFilePath = Path.Combine(_templatePath, $"{templateName}-email.html");
            
            if (System.IO.File.Exists(htmlFilePath))
            {
                var htmlContent = await System.IO.File.ReadAllTextAsync(htmlFilePath);
                
                var template = new EmailTemplate
                {
                    Name = templateName,
                    HtmlBody = htmlContent,
                    TextBody = ExtractTextFromHtml(htmlContent) // Simple text extraction
                };

                // Set subject based on template name
                template.Subject = templateName switch
                {
                    "welcome" => "🎉 Welcome to JCert Platform, {{Name}}!",
                    "password-reset" => "🔐 Reset Your JCert Password",
                    "password-changed" => "✅ Your JCert Password Has Been Changed",
                    _ => "JCert Platform Notification"
                };

                return template;
            }

            // Fallback to hardcoded templates if file doesn't exist
            var templates = new Dictionary<string, EmailTemplate>
            {
                ["welcome"] = new()
                {
                    Name = "welcome",
                    Subject = "Welcome to JCert Application, {{Name}}!",
                    HtmlBody = "<h1>Welcome {{Name}}!</h1><p>Thank you for joining our platform.</p><p>Your email: {{Email}}</p>",
                    TextBody = "Welcome {{Name}}! Thank you for joining our platform. Your email: {{Email}}"
                },
                ["password-reset"] = new()
                {
                    Name = "password-reset",
                    Subject = "🔐 Đặt lại mật khẩu JCertPre",
                    HtmlBody = @"
                        <!DOCTYPE html>
                        <html>
                        <head>
                            <meta charset='UTF-8'>
                            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                            <title>Password Reset</title>
                        </head>
                        <body style='margin: 0; padding: 0; background-color: #f4f4f4; font-family: Arial, sans-serif;'>
                            <div style='max-width: 600px; margin: 0 auto; background-color: white; border-radius: 8px; overflow: hidden; box-shadow: 0 2px 10px rgba(0,0,0,0.1);'>
                                <!-- Header -->
                                <div style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 40px 20px; text-align: center;'>
                                    <h1 style='color: white; margin: 0; font-size: 28px; font-weight: 600;'>🔐 Password Reset</h1>
                                    <p style='color: #e0e7ff; margin: 10px 0 0 0; font-size: 16px;'>Secure your JCert account</p>
                                </div>
                                
                                <!-- Content -->
                                <div style='padding: 40px 30px;'>
                                    <p style='font-size: 16px; color: #333; margin: 0 0 20px 0; line-height: 1.6;'>
                                        Hi <strong>{{Name}}</strong>,
                                    </p>
                                    
                                    <p style='font-size: 16px; color: #555; margin: 0 0 30px 0; line-height: 1.6;'>
                                        We received a request to reset your password for your JCert account. Click the button below to set a new password:
                                    </p>
                                    
                                    <!-- CTA Button -->
                                    <div style='text-align: center; margin: 40px 0;'>
                                        <a href='{{ResetLink}}' style='
                                            display: inline-block;
                                            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                                            color: white;
                                            padding: 16px 32px;
                                            text-decoration: none;
                                            border-radius: 50px;
                                            font-weight: 600;
                                            font-size: 16px;
                                            text-transform: uppercase;
                                            letter-spacing: 1px;
                                            box-shadow: 0 4px 15px rgba(102, 126, 234, 0.4);
                                            transition: all 0.3s ease;
                                        '>
                                            Reset My Password
                                        </a>
                                    </div>
                                    
                                    <!-- Security Info -->
                                    <div style='background-color: #fff3cd; border: 1px solid #ffeaa7; border-radius: 8px; padding: 20px; margin: 30px 0;'>
                                        <p style='color: #856404; margin: 0; font-size: 14px; line-height: 1.5;'>
                                            ⏰ <strong>This link expires in {{ExpiryMinutes}} minutes</strong> for security reasons.<br>
                                            🔒 The link can only be used once.
                                        </p>
                                    </div>
                                    
                                    <p style='font-size: 14px; color: #666; margin: 20px 0 0 0; line-height: 1.6;'>
                                        If you didn't request this password reset, please ignore this email. Your password will remain unchanged.
                                    </p>
                                    
                                    <p style='font-size: 14px; color: #666; margin: 10px 0 0 0; line-height: 1.6;'>
                                        If the button doesn't work, copy and paste this link into your browser:<br>
                                        <a href='{{ResetLink}}' style='color: #667eea; word-break: break-all;'>{{ResetLink}}</a>
                                    </p>
                                </div>
                                
                                <!-- Footer -->
                                <div style='background-color: #f8f9fa; padding: 30px; text-align: center; border-top: 1px solid #e9ecef;'>
                                    <p style='color: #6c757d; margin: 0; font-size: 12px; line-height: 1.5;'>
                                        This email was sent by JCert Platform<br>
                                        If you have any questions, please contact our support team.
                                    </p>
                                </div>
                            </div>
                        </body>
                        </html>",
                    TextBody = "Hi {{Name}}, click this link to reset your JCert password: {{ResetLink}} (expires in {{ExpiryMinutes}} minutes). If you didn't request this, please ignore this email."
                },
                ["password-changed"] = new()
                {
                    Name = "password-changed",
                    Subject = "✅ Mật khẩu JCertPre đã được thay đổi",
                    HtmlBody = @"
                        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                            <h1 style='color: #28a745; text-align: center;'>✅ Password Successfully Changed</h1>
                            <p>Hi {{Name}},</p>
                            <p>Your JCert password has been successfully changed.</p>
                            <p><strong>Details:</strong></p>
                            <ul>
                                <li>Time: {{ChangeTime}}</li>
                                <li>IP Address: {{IpAddress}}</li>
                            </ul>
                            <p>If you didn't make this change, please contact our support team immediately.</p>
                            <p>For security, all existing sessions have been logged out.</p>
                        </div>",
                    TextBody = "Hi {{Name}}, your password has been changed at {{ChangeTime}} from IP {{IpAddress}}. Contact support if this wasn't you."
                }
            };

            if (templates.TryGetValue(templateName.ToLower(), out var fallbackTemplate))
            {
                return fallbackTemplate;
            }

            throw new ArgumentException($"Email template '{templateName}' not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load email template '{TemplateName}'", templateName);
            throw;
        }
    }

    private static string ExtractTextFromHtml(string html)
    {
        // Simple HTML to text conversion - remove HTML tags
        var text = Regex.Replace(html, "<.*?>", string.Empty);
        // Clean up extra whitespace
        text = Regex.Replace(text, @"\s+", " ").Trim();
        return text;
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
