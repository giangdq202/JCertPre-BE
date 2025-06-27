using JCertPreApplication.Application.Dtos.Auth;
using JCertPreApplication.Application.Dtos.User;

namespace JCertPreApplication.Application.Features.Auth
{
    public interface IAuthService
    {
        Task<(string AccessToken, string RefreshToken, AppUserDto User)> LoginAsync(string username, string password);
        Task<(bool Succeeded, string AccessToken, string RefreshToken, AppUserDto User, string[] Errors)> RegisterAsync(RegisterModel model);
        Task<(string AccessToken, string RefreshToken, AppUserDto User)> RefreshTokenAsync(string refreshToken);
        Task<(string AccessToken, string RefreshToken, AppUserDto User)> FirebaseLoginAsync(string firebaseToken);
    }
}
