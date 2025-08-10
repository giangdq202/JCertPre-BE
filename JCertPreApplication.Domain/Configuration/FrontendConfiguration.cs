namespace JCertPreApplication.Domain.Configuration
{
    public class FrontendConfiguration
    {
        public const string SectionName = "Frontend";
        
        public string BaseUrl { get; set; } = string.Empty;
        public string PaymentSuccessEndpoint { get; set; } = string.Empty;
        public string PaymentCancelledEndpoint { get; set; } = string.Empty;
        public string PaymentErrorEndpoint { get; set; } = string.Empty;
        public string PaymentPendingEndpoint { get; set; } = string.Empty;
        public string ResetPasswordEndpoint { get; set; } = string.Empty;
        public string LoginEndpoint { get; set; } = string.Empty;
        
        // Computed properties
        public string PaymentSuccessUrl => $"{BaseUrl.TrimEnd('/')}{PaymentSuccessEndpoint}";
        public string PaymentCancelledUrl => $"{BaseUrl.TrimEnd('/')}{PaymentCancelledEndpoint}";
        public string PaymentErrorUrl => $"{BaseUrl.TrimEnd('/')}{PaymentErrorEndpoint}";
        public string PaymentPendingUrl => $"{BaseUrl.TrimEnd('/')}{PaymentPendingEndpoint}";
        public string ResetPasswordUrl => $"{BaseUrl.TrimEnd('/')}{ResetPasswordEndpoint}";
        public string LoginUrl => $"{BaseUrl.TrimEnd('/')}{LoginEndpoint}";
        
        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(BaseUrl))
                throw new InvalidOperationException("Frontend BaseUrl is required. Please configure Frontend__BaseUrl in your .env file.");
                
            if (!Uri.TryCreate(BaseUrl, UriKind.Absolute, out var uri))
                throw new InvalidOperationException("Frontend BaseUrl must be a valid URL.");
        }
    }
}
