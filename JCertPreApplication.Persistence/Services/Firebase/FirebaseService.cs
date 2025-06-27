using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Domain.Configuration;
using Microsoft.Extensions.Options;

namespace JCertPreApplication.Persistence.Services.Firebase
{
    public class FirebaseService : IFirebaseService
    {
        private readonly FirebaseAuth _firebaseAuth;

        public FirebaseService(IOptions<FirebaseConfiguration> firebaseConfig)
        {
            var config = firebaseConfig.Value;
            
            // Create service account JSON
            var serviceAccountJson = config.ToJson();
            var serviceAccountCredential = GoogleCredential.FromJson(serviceAccountJson);

            // Initialize Firebase Admin SDK if not already initialized
            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = serviceAccountCredential,
                    ProjectId = config.ProjectId
                });
            }

            _firebaseAuth = FirebaseAuth.DefaultInstance;
        }

        public async Task<FirebaseToken?> VerifyTokenAsync(string firebaseToken)
        {
            try
            {
                var decodedToken = await _firebaseAuth.VerifyIdTokenAsync(firebaseToken);
                return decodedToken;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<(string Email, string Name, string? Picture)> GetUserInfoFromTokenAsync(string firebaseToken)
        {
            var decodedToken = await VerifyTokenAsync(firebaseToken);
            
            if (decodedToken == null)
                throw new UnauthorizedAccessException("Invalid Firebase token");

            var email = decodedToken.Claims.TryGetValue("email", out var emailClaim) 
                ? emailClaim.ToString() ?? string.Empty 
                : string.Empty;

            var name = decodedToken.Claims.TryGetValue("name", out var nameClaim) 
                ? nameClaim.ToString() ?? string.Empty 
                : string.Empty;

            var picture = decodedToken.Claims.TryGetValue("picture", out var pictureClaim) 
                ? pictureClaim.ToString() 
                : null;

            return (email, name, picture);
        }
    }
} 