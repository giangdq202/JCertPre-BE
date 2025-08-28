using JCertPreApplication.Application.Dtos.Auth;
using JCertPreApplication.Application.Features.Auth;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
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
        [AllowAnonymous]
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
        [AllowAnonymous]
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
        [AllowAnonymous]
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
        [AllowAnonymous]
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
        [AllowAnonymous]
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
        [AllowAnonymous]
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
        [AllowAnonymous]
        public async Task<IActionResult> ValidateRefreshToken([FromBody] ValidateRefreshTokenModel model)
        {
            var isValid = await _authService.ValidateRefreshTokenAsync(model.RefreshToken);
            return Ok(new { 
                isValid = isValid,
                message = isValid ? "Refresh token is valid" : "Refresh token is invalid or not in whitelist"
            });
        }

        /// <summary>
        /// Initiates password reset process by sending reset token via email.
        /// </summary>
        /// <param name="model">The email address to send reset instructions to.</param>
        /// <returns>Success message (generic for security).</returns>
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest model)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var message = await _authService.ForgotPasswordAsync(model.Email, ipAddress);
            return Ok(new { message });
        }

        /// <summary>
        /// Resets user password using valid reset token from Redis cache.
        /// </summary>
        /// <param name="model">Reset token and new password information.</param>
        /// <returns>Success message.</returns>
        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest model)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var message = await _authService.ResetPasswordAsync(model.Token, model.NewPassword, ipAddress);
            return Ok(new { message });
        }

        /// <summary>
        /// Validates a password reset token stored in Redis cache.
        /// </summary>
        /// <param name="token">The reset token to validate.</param>
        /// <returns>The validation result.</returns>
        [HttpGet("validate-reset-token/{token}")]
        [AllowAnonymous]
        public async Task<IActionResult> ValidateResetToken(string token)
        {
            var isValid = await _authService.ValidateResetTokenAsync(token);
            return Ok(new { 
                isValid = isValid,
                message = isValid ? "Token đặt lại mật khẩu hợp lệ" : "Token đặt lại mật khẩu không hợp lệ, đã hết hạn hoặc đã được sử dụng"
            });
        }
    }
}
