using FirebaseAdmin.Auth;

namespace JCertPreApplication.Application.Contracts
{
    public interface IFirebaseService
    {
        Task<FirebaseToken?> VerifyTokenAsync(string firebaseToken);
        Task<(string Email, string Name, string? Picture)> GetUserInfoFromTokenAsync(string firebaseToken);
    }
} 