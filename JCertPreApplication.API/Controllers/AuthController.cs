using JCertPreApplication.API.Common;
using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.Auth;
using JCertPreApplication.Application.Features.Auth;
using Microsoft.AspNetCore.Mvc;

namespace JCertPreApplication.API.Controllers
{
    /// <summary>
    /// Handles user authentication, including registration, login, and JWT token management.
    /// </summary>
    [Route("api/auth")]
    [ApiController]
    [Tags("Authentication")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        /// <summary>
        /// Registers a new user and returns JWT tokens.
        /// </summary>
        /// <param name="model">User registration information.</param>
        /// <returns>Authentication tokens and user profile.</returns>
        [HttpPost("register")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Register([FromForm] RegisterModel model)
        {
            var (accessToken, refreshToken, user) = await _authService.RegisterAsync(model);
            return Ok(new { accessToken, refreshToken, user });
        }

        /// <summary>
        /// Authenticates a user via email and password.
        /// </summary>
        /// <param name="model">User login credentials.</param>
        /// <returns>Authentication tokens and user profile.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var (accessToken, refreshToken, user) = await _authService.LoginAsync(model.Email, model.Password);
            return Ok(new { accessToken, refreshToken, user });
        }

        /// <summary>
        /// Authenticates a user via Firebase ID token.
        /// </summary>
        /// <param name="model">Firebase ID token for social login.</param>
        /// <returns>Authentication tokens and user profile.</returns>
        [HttpPost("firebase-login")]
        public async Task<IActionResult> FirebaseLogin([FromBody] FirebaseLoginModel model)
        {
            var (accessToken, refreshToken, user) = await _authService.FirebaseLoginAsync(model.FirebaseToken);
            return Ok(new { accessToken, refreshToken, user });
        }

        /// <summary>
        /// Issues a new token pair using a valid refresh token.
        /// </summary>
        /// <param name="model">The current access and refresh token pair.</param>
        /// <returns>A new token pair and user profile.</returns>
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenModel model)
        {
            var (accessToken, newRefreshToken, user) = await _authService.RefreshTokenAsync(model.AccessToken, model.RefreshToken);
            return Ok(new { accessToken, refreshToken = newRefreshToken, user });
        }

        /// <summary>
        /// Securely logs out the user by revoking their tokens.
        /// </summary>
        /// <param name="model">The access and refresh tokens to revoke.</param>
        /// <returns>A confirmation message.</returns>
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutModel model)
        {
            await _authService.LogoutAsync(model.AccessToken, model.RefreshToken);
            return Ok(new { message = "Logged out successfully" });
        }

        /// <summary>
        /// Validates an access token.
        /// </summary>
        /// <param name="model">The access token to validate.</param>
        /// <returns>The validation result.</returns>
        [HttpPost("validate-access-token")]
        public async Task<IActionResult> ValidateAccessToken([FromBody] ValidateAccessTokenModel model)
        {
            var isValid = await _authService.ValidateAccessTokenAsync(model.AccessToken);
            return Ok(new { 
                isValid = isValid,
                message = isValid ? "Access token is valid" : "Access token is invalid or revoked"
            });
        }

        /// <summary>
        /// Validates a refresh token.
        /// </summary>
        /// <param name="model">The refresh token to validate.</param>
        /// <returns>The validation result.</returns>
        [HttpPost("validate-refresh-token")]
        public async Task<IActionResult> ValidateRefreshToken([FromBody] ValidateRefreshTokenModel model)
        {
            var isValid = await _authService.ValidateRefreshTokenAsync(model.RefreshToken);
            return Ok(new { 
                isValid = isValid,
                message = isValid ? "Refresh token is valid" : "Refresh token is invalid or not in whitelist"
            });
        }
    }
}
