using JCertPreApplication.Domain.Configuration;
using System.Text.Json;

namespace JCertPreApplication.Persistence.Services.Firebase
{
    public static class FirebaseConfigurationHelper
    {
        public static string ToJson(this FirebaseConfiguration config)
        {
            // Fix the private key by replacing literal \n with actual newlines
            var fixedPrivateKey = config.PrivateKey?.Replace("\\n", "\n") ?? string.Empty;
            
            var serviceAccountObject = new
            {
                type = config.Type,
                project_id = config.ProjectId,
                private_key_id = config.PrivateKeyId,
                private_key = fixedPrivateKey,
                client_email = config.ClientEmail,
                client_id = config.ClientId,
                auth_uri = config.AuthUri,
                token_uri = config.TokenUri,
                auth_provider_x509_cert_url = config.AuthProviderX509CertUrl,
                client_x509_cert_url = config.ClientX509CertUrl,
                universe_domain = config.UniverseDomain
            };
            
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            
            return JsonSerializer.Serialize(serviceAccountObject, options);
        }
    }
} 