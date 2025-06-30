using JCertPreApplication.Application.Dtos.Auth;
using JCertPreApplication.Application.Dtos.User;

namespace JCertPreApplication.Application.Features.Auth
{
    public interface IAuthService
    {
        Task<(string AccessToken, string RefreshToken, AppUserDto User)> LoginAsync(string email, string password);
        Task<(string AccessToken, string RefreshToken, AppUserDto User)> RegisterAsync(RegisterModel model);
        Task<(string AccessToken, string RefreshToken, AppUserDto User)> RefreshTokenAsync(string refreshToken);
        Task<(string AccessToken, string RefreshToken, AppUserDto User)> FirebaseLoginAsync(string firebaseToken);
        Task LogoutAsync(string accessToken, string refreshToken);
        
        // Token validation methods
        Task<bool> ValidateAccessTokenAsync(string accessToken);
        Task<bool> ValidateRefreshTokenAsync(string refreshToken);
    }
}
