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
    
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(ClientId))
            throw new InvalidOperationException("PayOS ClientId is required. Please configure PayOS__ClientId in your .env file.");
            
        if (string.IsNullOrWhiteSpace(ApiKey))
            throw new InvalidOperationException("PayOS ApiKey is required. Please configure PayOS__ApiKey in your .env file.");
            
        if (string.IsNullOrWhiteSpace(ChecksumKey))
            throw new InvalidOperationException("PayOS ChecksumKey is required. Please configure PayOS__ChecksumKey in your .env file.");
            
        if (string.IsNullOrWhiteSpace(ReturnEndpoint))
            throw new InvalidOperationException("PayOS ReturnEndpoint is required. Please configure PayOS__ReturnEndpoint in your .env file.");
            
        if (string.IsNullOrWhiteSpace(CancelEndpoint))
            throw new InvalidOperationException("PayOS CancelEndpoint is required. Please configure PayOS__CancelEndpoint in your .env file.");
    }
}
