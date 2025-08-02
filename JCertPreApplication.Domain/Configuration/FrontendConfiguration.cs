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
        
        // Computed properties
        public string PaymentSuccessUrl => $"{BaseUrl.TrimEnd('/')}{PaymentSuccessEndpoint}";
        public string PaymentCancelledUrl => $"{BaseUrl.TrimEnd('/')}{PaymentCancelledEndpoint}";
        public string PaymentErrorUrl => $"{BaseUrl.TrimEnd('/')}{PaymentErrorEndpoint}";
        public string PaymentPendingUrl => $"{BaseUrl.TrimEnd('/')}{PaymentPendingEndpoint}";
    }
}
