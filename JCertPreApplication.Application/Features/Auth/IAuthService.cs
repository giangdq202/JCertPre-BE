using JCertPreApplication.Application.Dtos.Auth;
using JCertPreApplication.Application.Dtos.User;
using JCertPreApplication.Application.Exceptions;

namespace JCertPreApplication.Application.Features.Auth
{
    /// <summary>
    /// Service interface for authentication operations including login, registration, token management and validation
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Authenticates user with email and password
        /// </summary>
        /// <param name="email">User email address</param>
        /// <param name="password">User password</param>
        /// <returns>Tuple containing access token, refresh token, and user information</returns>
        /// <exception cref="ApiException">Thrown when credentials are invalid or account is inactive</exception>
        Task<(string AccessToken, string RefreshToken, AppUserDto User)> LoginAsync(string email, string password);
        
        /// <summary>
        /// Registers a new user account
        /// </summary>
        /// <param name="model">Registration data containing user information</param>
        /// <returns>Tuple containing access token, refresh token, and user information</returns>
        /// <exception cref="ApiException">Thrown when email already exists or validation fails</exception>
        Task<(string AccessToken, string RefreshToken, AppUserDto User)> RegisterAsync(RegisterModel model);
        
        /// <summary>
        /// Refreshes expired access token using valid refresh token with token rotation security
        /// </summary>
        /// <param name="accessToken">Current access token (will be revoked immediately)</param>
        /// <param name="refreshToken">Valid refresh token (will be rotated)</param>
        /// <returns>Tuple containing new access token, new refresh token, and user information</returns>
        /// <exception cref="ApiException">Thrown when tokens are invalid, expired, or don't belong to same user</exception>
        Task<(string AccessToken, string RefreshToken, AppUserDto User)> RefreshTokenAsync(string accessToken, string refreshToken);
        
        /// <summary>
        /// Authenticates user using Firebase token (social login)
        /// </summary>
        /// <param name="firebaseToken">Valid Firebase authentication token</param>
        /// <returns>Tuple containing access token, refresh token, and user information</returns>
        /// <exception cref="ApiException">Thrown when Firebase token is invalid or authentication fails</exception>
        Task<(string AccessToken, string RefreshToken, AppUserDto User)> FirebaseLoginAsync(string firebaseToken);
        
        /// <summary>
        /// Logs out user by revoking both access and refresh tokens
        /// </summary>
        /// <param name="accessToken">Access token to be revoked (added to blacklist)</param>
        /// <param name="refreshToken">Refresh token to be revoked (removed from whitelist)</param>
        /// <exception cref="ApiException">Thrown when tokens are malformed</exception>
        Task LogoutAsync(string accessToken, string refreshToken);
        
        /// <summary>
        /// Validates access token including signature, expiration, and revocation status
        /// </summary>
        /// <param name="accessToken">Access token to validate</param>
        /// <returns>True if token is valid and not revoked, false otherwise</returns>
        Task<bool> ValidateAccessTokenAsync(string accessToken);
        
        /// <summary>
        /// Validates refresh token including signature, expiration, and whitelist status
        /// </summary>
        /// <param name="refreshToken">Refresh token to validate</param>
        /// <returns>True if token is valid and in whitelist, false otherwise</returns>
        Task<bool> ValidateRefreshTokenAsync(string refreshToken);
    }
}
