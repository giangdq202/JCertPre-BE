using JCertPreApplication.Application.Dtos.Auth;
using JCertPreApplication.Application.Dtos.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCertPreApplication.Application.Features.Auth
{
    public interface IAuthService
    {
        Task<(string AccessToken, string RefreshToken, AppUserDto User)> LoginAsync(string username, string password);
        Task<(bool Succeeded, string AccessToken, string RefreshToken, AppUserDto User, string[] Errors)> RegisterAsync(RegisterModel model);
        Task<(string AccessToken, string RefreshToken, AppUserDto User)> RefreshTokenAsync(string refreshToken);
    }
}
