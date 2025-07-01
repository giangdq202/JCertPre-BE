using JCertPreApplication.Application.Dtos.Auth;
using JCertPreApplication.Application.Features.Auth;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace JCertPreApplication.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        /// <summary>
        /// Register a new user account
        /// </summary>
        /// <param name="model">User registration information</param>
        /// <returns>Authentication tokens and user information</returns>
        /// <response code="200">Registration successful</response>
        /// <response code="400">Invalid input data or email already exists</response>
        [HttpPost("register")]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            // Model validation is handled by middleware if using [Required] attributes
            // Or we can throw ApiException here if needed
            
            var (accessToken, refreshToken, user) = await _authService.RegisterAsync(model);
            return Ok(new { accessToken, refreshToken, user });
        }

        /// <summary>
        /// Authenticate user with email and password
        /// </summary>
        /// <param name="model">Login credentials</param>
        /// <returns>Authentication tokens and user information</returns>
        /// <response code="200">Login successful</response>
        /// <response code="401">Invalid credentials</response>
        /// <response code="403">Account inactive</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.Forbidden)]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var (accessToken, refreshToken, user) = await _authService.LoginAsync(model.Email, model.Password);
            return Ok(new { accessToken, refreshToken, user });
        }

        /// <summary>
        /// Authenticate user using Firebase token (social login)
        /// </summary>
        /// <param name="model">Firebase authentication token</param>
        /// <returns>Authentication tokens and user information</returns>
        /// <response code="200">Firebase login successful</response>
        /// <response code="401">Invalid Firebase token</response>
        /// <response code="500">Firebase service error</response>
        [HttpPost("firebase-login")]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> FirebaseLogin([FromBody] FirebaseLoginModel model)
        {
            var (accessToken, refreshToken, user) = await _authService.FirebaseLoginAsync(model.FirebaseToken);
            return Ok(new { accessToken, refreshToken, user });
        }

        /// <summary>
        /// Refresh expired access token using valid refresh token
        /// </summary>
        /// <param name="model">Current access and refresh tokens</param>
        /// <returns>New authentication tokens and user information</returns>
        /// <response code="200">Token refresh successful</response>
        /// <response code="400">Invalid token format or token mismatch</response>
        /// <response code="401">Invalid or expired tokens</response>
        [HttpPost("refresh")]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenModel model)
        {
            var (accessToken, newRefreshToken, user) = await _authService.RefreshTokenAsync(model.AccessToken, model.RefreshToken);
            return Ok(new { accessToken, refreshToken = newRefreshToken, user });
        }

        /// <summary>
        /// Logout user by revoking access and refresh tokens
        /// </summary>
        /// <param name="model">Tokens to be revoked</param>
        /// <returns>Logout confirmation</returns>
        /// <response code="200">Logout successful</response>
        /// <response code="400">Invalid token format</response>
        /// <response code="500">Logout process error</response>
        [HttpPost("logout")]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Logout([FromBody] LogoutModel model)
        {
            await _authService.LogoutAsync(model.AccessToken, model.RefreshToken);
            return Ok(new { message = "Logged out successfully" });
        }

        /// <summary>
        /// Validate access token including signature, expiration and revocation status
        /// </summary>
        /// <param name="model">Access token to validate</param>
        /// <returns>Token validation result</returns>
        /// <response code="200">Validation completed</response>
        [HttpPost("validate-access-token")]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ValidateAccessToken([FromBody] ValidateAccessTokenModel model)
        {
            var isValid = await _authService.ValidateAccessTokenAsync(model.AccessToken);
            return Ok(new { 
                isValid = isValid,
                message = isValid ? "Access token is valid" : "Access token is invalid or revoked"
            });
        }

        /// <summary>
        /// Validate refresh token including signature, expiration and whitelist status
        /// </summary>
        /// <param name="model">Refresh token to validate</param>
        /// <returns>Token validation result</returns>
        /// <response code="200">Validation completed</response>
        [HttpPost("validate-refresh-token")]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
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
