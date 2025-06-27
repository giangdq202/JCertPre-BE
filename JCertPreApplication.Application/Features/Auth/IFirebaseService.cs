using FirebaseAdmin.Auth;

namespace JCertPreApplication.Application.Features.Auth
{
    public interface IFirebaseService
    {
        Task<FirebaseToken?> VerifyTokenAsync(string firebaseToken);
        Task<(string Email, string Name, string? Picture)> GetUserInfoFromTokenAsync(string firebaseToken);
    }
} 