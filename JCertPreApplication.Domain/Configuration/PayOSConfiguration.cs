namespace JCertPreApplication.Domain.Configuration;

public class PayOSConfiguration
{
    public const string SectionName = "PayOS";
    
    public string ClientId { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string ChecksumKey { get; set; } = string.Empty;
    public string ReturnEndpoint { get; set; } = string.Empty;
    public string CancelEndpoint { get; set; } = string.Empty;
    
    // BaseUrl sẽ được inject từ ApiConfiguration.PublicUrl (external URL for callbacks)
    private string? _baseUrl;
    public string BaseUrl 
    { 
        get => _baseUrl ?? string.Empty;
        set => _baseUrl = value;
    }
    
    // Computed properties
    public string ReturnUrl => $"{BaseUrl.TrimEnd('/')}{ReturnEndpoint}";
    public string CancelUrl => $"{BaseUrl.TrimEnd('/')}{CancelEndpoint}";
}
