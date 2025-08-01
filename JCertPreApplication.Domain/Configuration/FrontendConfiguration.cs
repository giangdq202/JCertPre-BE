namespace JCertPreApplication.Domain.Configuration
{
    public class FrontendConfiguration
    {
        public const string SectionName = "Frontend";
        
        public string BaseUrl { get; set; } = string.Empty;
        public string PaymentSuccessUrl { get; set; } = string.Empty;
        public string PaymentCancelledUrl { get; set; } = string.Empty;
        public string PaymentErrorUrl { get; set; } = string.Empty;
        public string PaymentPendingUrl { get; set; } = string.Empty;
    }
}
