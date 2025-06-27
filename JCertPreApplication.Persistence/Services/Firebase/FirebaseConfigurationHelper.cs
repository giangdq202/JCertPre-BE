using JCertPreApplication.Domain.Configuration;

namespace JCertPreApplication.Persistence.Services.Firebase
{
    public static class FirebaseConfigurationHelper
    {
        public static string ToJson(this FirebaseConfiguration config)
        {
            return $@"{{
  ""type"": ""{config.Type}"",
  ""project_id"": ""{config.ProjectId}"",
  ""private_key_id"": ""{config.PrivateKeyId}"",
  ""private_key"": ""{config.PrivateKey.Replace("\n", "\\n")}"",
  ""client_email"": ""{config.ClientEmail}"",
  ""client_id"": ""{config.ClientId}"",
  ""auth_uri"": ""{config.AuthUri}"",
  ""token_uri"": ""{config.TokenUri}"",
  ""auth_provider_x509_cert_url"": ""{config.AuthProviderX509CertUrl}"",
  ""client_x509_cert_url"": ""{config.ClientX509CertUrl}"",
  ""universe_domain"": ""{config.UniverseDomain}""
}}";
        }
    }
} 