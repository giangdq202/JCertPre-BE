using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Domain.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JCertPreApplication.Persistence.Services.Firebase
{
    public class FirebaseService : IFirebaseService
    {
        private readonly FirebaseAuth _firebaseAuth;
        private readonly ILogger<FirebaseService> _logger;

        public FirebaseService(IOptions<FirebaseConfiguration> firebaseConfig, ILogger<FirebaseService> logger)
        {
            _logger = logger;
            
            try
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
                    
                    _logger.LogInformation("Firebase Admin SDK initialized successfully for project: {ProjectId}", config.ProjectId);
                }

                _firebaseAuth = FirebaseAuth.DefaultInstance;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize Firebase Admin SDK");
                throw new InvalidOperationException("Failed to initialize Firebase Admin SDK. Please check your Firebase configuration.", ex);
            }
        }

        public async Task<FirebaseToken?> VerifyTokenAsync(string firebaseToken)
        {
            try
            {
                if (string.IsNullOrEmpty(firebaseToken))
                {
                    _logger.LogWarning("Firebase token verification failed: token is null or empty");
                    return null;
                }

                var decodedToken = await _firebaseAuth.VerifyIdTokenAsync(firebaseToken);
                _logger.LogDebug("Firebase token verified successfully for user: {UserId}", decodedToken.Uid);
                return decodedToken;
            }
            catch (FirebaseAuthException ex)
            {
                _logger.LogWarning(ex, "Firebase token verification failed: {ErrorCode} - {ErrorMessage}", ex.ErrorCode, ex.Message);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during Firebase token verification");
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