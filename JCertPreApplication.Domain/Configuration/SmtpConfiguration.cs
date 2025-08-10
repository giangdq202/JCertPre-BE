namespace JCertPreApplication.Domain.Configuration;

public class SmtpConfiguration
{
    public const string SectionName = "Smtp";
    
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 587;
    public bool EnableSsl { get; set; } = true;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FromEmail { get; set; } = string.Empty;
    public string FromName { get; set; } = string.Empty;
    public int Timeout { get; set; } = 30000;
    
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Host))
            throw new InvalidOperationException("SMTP Host is required. Please configure Smtp__Host in your .env file.");
            
        if (Port <= 0 || Port > 65535)
            throw new InvalidOperationException("SMTP Port must be between 1 and 65535. Please configure Smtp__Port in your .env file.");
            
        if (string.IsNullOrWhiteSpace(Username))
            throw new InvalidOperationException("SMTP Username is required. Please configure Smtp__Username in your .env file.");
            
        if (string.IsNullOrWhiteSpace(Password))
            throw new InvalidOperationException("SMTP Password is required. Please configure Smtp__Password in your .env file.");
            
        if (string.IsNullOrWhiteSpace(FromEmail))
            throw new InvalidOperationException("SMTP FromEmail is required. Please configure Smtp__FromEmail in your .env file.");
            
        if (string.IsNullOrWhiteSpace(FromName))
            throw new InvalidOperationException("SMTP FromName is required. Please configure Smtp__FromName in your .env file.");
            
        if (Timeout <= 0)
            throw new InvalidOperationException("SMTP Timeout must be greater than 0. Please configure Smtp__Timeout in your .env file.");
    }
}
