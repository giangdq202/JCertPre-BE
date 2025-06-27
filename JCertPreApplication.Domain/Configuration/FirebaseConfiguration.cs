namespace JCertPreApplication.Domain.Configuration
{
    public class FirebaseConfiguration
    {
        public const string SectionName = "Firebase";

        public string Type { get; set; } = string.Empty;
        public string ProjectId { get; set; } = string.Empty;
        public string PrivateKeyId { get; set; } = string.Empty;
        public string PrivateKey { get; set; } = string.Empty;
        public string ClientEmail { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string AuthUri { get; set; } = string.Empty;
        public string TokenUri { get; set; } = string.Empty;
        public string AuthProviderX509CertUrl { get; set; } = string.Empty;
        public string ClientX509CertUrl { get; set; } = string.Empty;
        public string UniverseDomain { get; set; } = string.Empty;

        public string ToJson()
        {
            return $@"{{
  ""type"": ""{Type}"",
  ""project_id"": ""{ProjectId}"",
  ""private_key_id"": ""{PrivateKeyId}"",
  ""private_key"": ""{PrivateKey.Replace("\n", "\\n")}"",
  ""client_email"": ""{ClientEmail}"",
  ""client_id"": ""{ClientId}"",
  ""auth_uri"": ""{AuthUri}"",
  ""token_uri"": ""{TokenUri}"",
  ""auth_provider_x509_cert_url"": ""{AuthProviderX509CertUrl}"",
  ""client_x509_cert_url"": ""{ClientX509CertUrl}"",
  ""universe_domain"": ""{UniverseDomain}""
}}";
        }
    }
} 