using JCertPreApplication.Application.Dtos.Auth;

namespace JCertPreApplication.Application.Contracts
{
    public interface IFirebaseService
    {
        Task<FirebaseTokenDto?> VerifyTokenAsync(string firebaseToken);
        Task<(string Email, string Name, string? Picture)> GetUserInfoFromTokenAsync(string firebaseToken);
    }
} 