using JCertPreApplication.API.Common;
using JCertPreApplication.Application.Dtos.Auth;
using JCertPreApplication.Application.Features.Auth;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace JCertPreApplication.API.Controllers
{
    /// <summary>
    /// Authentication and Authorization API Controller
    /// </summary>
    /// <remarks>
    /// Provides comprehensive authentication and authorization functionality including:
    /// - User registration and account creation
    /// - Email/password authentication
    /// - Firebase social authentication (Google, Facebook, etc.)
    /// - JWT token management (access/refresh tokens)
    /// - Token validation and revocation
    /// - Secure logout with token cleanup
    /// 
    /// Security Features:
    /// - JWT-based authentication with access/refresh token pattern
    /// - Token revocation and blacklisting
    /// - Firebase integration for social login
    /// - Secure password hashing
    /// - Account status validation
    /// 
    /// Token Lifecycle:
    /// 1. Login/Register → Get access + refresh tokens
    /// 2. Use access token for API calls
    /// 3. When access token expires → Use refresh token to get new tokens
    /// 4. Logout → Revoke both tokens
    /// </remarks>
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
        /// Register a new user account
        /// </summary>
        /// <remarks>
        /// Creates a new user account with the provided information.
        /// 
        /// Registration Process:
        /// 1. Validates input data (email format, password strength, etc.)
        /// 2. Checks if email is already registered
        /// 3. Creates user account with hashed password
        /// 4. Assigns default role and permissions
        /// 5. Generates JWT tokens for immediate login
        /// 
        /// Password Requirements:
        /// - Minimum 6 characters
        /// - Maximum 100 characters
        /// - Should include mix of letters, numbers, and special characters
        /// 
        /// Important Notes:
        /// - Email addresses are case-insensitive and must be unique
        /// - New accounts are created with 'Active' status by default
        /// - Returns both access and refresh tokens for immediate authentication
        /// - User receives full name, email, and basic profile information
        /// 
        /// Security:
        /// - Passwords are hashed using secure algorithms
        /// - Email uniqueness is enforced at database level
        /// - Input validation prevents SQL injection and XSS
        /// </remarks>
        /// <param name="model">User registration information including email, password, and profile data</param>
        /// <returns>Authentication tokens and user profile information</returns>
        /// <response code="200">Registration successful. Returns access token, refresh token, and user information.</response>
        /// <response code="400">Invalid input data. Common issues: invalid email format, weak password, missing required fields.</response>
        /// <response code="409">Conflict. Email address is already registered.</response>
        /// <response code="500">Internal server error during registration process.</response>
        [HttpPost("register")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
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
        /// <remarks>
        /// Authenticates user using email and password credentials.
        /// 
        /// Authentication Process:
        /// 1. Validates email format and checks if user exists
        /// 2. Verifies password against stored hash
        /// 3. Checks account status (active/inactive/suspended)
        /// 4. Generates new JWT access and refresh tokens
        /// 5. Returns tokens and user profile information
        /// 
        /// Token Information:
        /// - Access Token: Short-lived (typically 15-60 minutes), used for API authentication
        /// - Refresh Token: Long-lived (typically 7-30 days), used to get new access tokens
        /// 
        /// Security Features:
        /// - Password verification using secure hashing
        /// - Account status validation
        /// - Failed login attempt tracking (future enhancement)
        /// - Secure token generation with user-specific claims
        /// 
        /// Account Status Handling:
        /// - Active: Login allowed, returns tokens
        /// - Inactive: Login denied with 403 Forbidden
        /// - Suspended: Login denied with specific error message
        /// </remarks>
        /// <param name="model">Login credentials containing email and password</param>
        /// <returns>Authentication tokens and user profile information</returns>
        /// <response code="200">Login successful. Returns access token, refresh token, and user information.</response>
        /// <response code="400">Invalid request format or missing credentials.</response>
        /// <response code="401">Invalid credentials. Email not found or password incorrect.</response>
        /// <response code="403">Account inactive or suspended. Login not allowed.</response>
        /// <response code="500">Internal server error during authentication process.</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var (accessToken, refreshToken, user) = await _authService.LoginAsync(model.Email, model.Password);
            return Ok(new { accessToken, refreshToken, user });
        }

        /// <summary>
        /// Authenticate user using Firebase token (Social Login)
        /// </summary>
        /// <remarks>
        /// Authenticates user using Firebase authentication token from social providers.
        /// 
        /// Supported Social Providers:
        /// - Google (recommended)
        /// - Facebook
        /// - Twitter
        /// - GitHub
        /// - Apple (iOS)
        /// 
        /// Firebase Authentication Flow:
        /// 1. Client authenticates with social provider via Firebase SDK
        /// 2. Client receives Firebase ID token
        /// 3. Client sends Firebase token to this endpoint
        /// 4. Server verifies token with Firebase Admin SDK
        /// 5. Server creates/updates user account and returns JWT tokens
        /// 
        /// User Account Handling:
        /// - New User: Automatically creates account with social provider information
        /// - Existing User: Links social account to existing email if matches
        /// - Profile Data: Uses social provider data (name, email, profile picture)
        /// 
        /// Security Benefits:
        /// - No password storage required
        /// - Leverages provider's security infrastructure
        /// - Firebase token verification ensures authenticity
        /// - Automatic account creation reduces friction
        /// 
        /// Token Verification:
        /// - Firebase Admin SDK verifies token signature
        /// - Checks token expiration and issuer
        /// - Validates audience and project ID
        /// - Extracts verified user information
        /// </remarks>
        /// <param name="model">Firebase authentication token from social login</param>
        /// <returns>Authentication tokens and user profile information</returns>
        /// <response code="200">Firebase login successful. Returns access token, refresh token, and user information.</response>
        /// <response code="400">Invalid Firebase token format or missing token.</response>
        /// <response code="401">Invalid or expired Firebase token. Token verification failed.</response>
        /// <response code="500">Firebase service error or internal server error during authentication.</response>
        [HttpPost("firebase-login")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> FirebaseLogin([FromBody] FirebaseLoginModel model)
        {
            var (accessToken, refreshToken, user) = await _authService.FirebaseLoginAsync(model.FirebaseToken);
            return Ok(new { accessToken, refreshToken, user });
        }

        /// <summary>
        /// Refresh expired access token
        /// </summary>
        /// <remarks>
        /// Refreshes an expired access token using a valid refresh token.
        /// 
        /// Token Refresh Process:
        /// 1. Validates refresh token signature and expiration
        /// 2. Checks if refresh token is in whitelist (not revoked)
        /// 3. Verifies token belongs to the same user as access token
        /// 4. Generates new access token with updated expiration
        /// 5. Optionally rotates refresh token for enhanced security
        /// 
        /// Security Considerations:
        /// - Refresh tokens are stored in secure whitelist
        /// - Token binding prevents token theft attacks
        /// - Old refresh token may be invalidated (rotation)
        /// - User information is re-validated during refresh
        /// 
        /// When to Use:
        /// - When access token expires (typically 15-60 minutes)
        /// - Before making API calls with expired tokens
        /// - As part of automatic token renewal in client applications
        /// 
        /// Error Handling:
        /// - Invalid tokens require full re-authentication
        /// - Expired refresh tokens require user login
        /// - Token mismatch indicates potential security issue
        /// 
        /// Best Practices:
        /// - Implement automatic refresh in client applications
        /// - Store refresh tokens securely (HttpOnly cookies recommended)
        /// - Handle refresh failures gracefully with re-login prompts
        /// </remarks>
        /// <param name="model">Current access token and refresh token pair</param>
        /// <returns>New authentication tokens and updated user information</returns>
        /// <response code="200">Token refresh successful. Returns new access token, refresh token, and user information.</response>
        /// <response code="400">Invalid token format or token mismatch between access and refresh tokens.</response>
        /// <response code="401">Invalid, expired, or revoked tokens. Re-authentication required.</response>
        /// <response code="500">Internal server error during token refresh process.</response>
        [HttpPost("refresh")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenModel model)
        {
            var (accessToken, newRefreshToken, user) = await _authService.RefreshTokenAsync(model.AccessToken, model.RefreshToken);
            return Ok(new { accessToken, refreshToken = newRefreshToken, user });
        }

        /// <summary>
        /// Logout user and revoke tokens
        /// </summary>
        /// <remarks>
        /// Securely logs out user by revoking access and refresh tokens.
        /// 
        /// Logout Process:
        /// 1. Adds access token to revocation blacklist
        /// 2. Removes refresh token from whitelist
        /// 3. Invalidates all active sessions for the user
        /// 4. Clears any cached user data
        /// 5. Returns confirmation of successful logout
        /// 
        /// Security Benefits:
        /// - Prevents token reuse after logout
        /// - Immediately invalidates all user sessions
        /// - Protects against token theft scenarios
        /// - Ensures clean session termination
        /// 
        /// Token Revocation:
        /// - Access Token: Added to blacklist, checked on each API call
        /// - Refresh Token: Removed from whitelist, cannot be used for refresh
        /// - Session Data: Cleared from server-side storage
        /// 
        /// Client Responsibilities:
        /// - Clear tokens from client storage (localStorage, cookies, etc.)
        /// - Redirect user to login page
        /// - Clear any cached user data
        /// - Stop any automatic refresh timers
        /// 
        /// Use Cases:
        /// - User-initiated logout
        /// - Security-required logout (password change, etc.)
        /// - Administrative session termination
        /// - Automatic logout on token compromise detection
        /// </remarks>
        /// <param name="model">Access and refresh tokens to be revoked</param>
        /// <returns>Logout confirmation message</returns>
        /// <response code="200">Logout successful. Tokens have been revoked and session terminated.</response>
        /// <response code="400">Invalid token format or missing required tokens.</response>
        /// <response code="500">Internal server error during logout process. Tokens may still be valid.</response>
        [HttpPost("logout")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Logout([FromBody] LogoutModel model)
        {
            await _authService.LogoutAsync(model.AccessToken, model.RefreshToken);
            return Ok(new { message = "Logged out successfully" });
        }

        /// <summary>
        /// Validate access token
        /// </summary>
        /// <remarks>
        /// Validates an access token's signature, expiration, and revocation status.
        /// 
        /// Validation Checks:
        /// 1. Signature Verification: Ensures token wasn't tampered with
        /// 2. Expiration Check: Verifies token is still within validity period
        /// 3. Revocation Status: Checks if token is in blacklist (revoked)
        /// 4. Format Validation: Ensures proper JWT structure
        /// 5. Claims Validation: Verifies issuer, audience, and other claims
        /// 
        /// Use Cases:
        /// - Client-side token validation before API calls
        /// - Third-party service token verification
        /// - Debugging authentication issues
        /// - Security auditing and monitoring
        /// 
        /// Response Information:
        /// - isValid: Boolean indicating overall token validity
        /// - message: Human-readable explanation of validation result
        /// 
        /// Token States:
        /// - Valid: Token passes all validation checks
        /// - Expired: Token signature is valid but expired
        /// - Revoked: Token is in blacklist (user logged out)
        /// - Invalid: Token signature or format is incorrect
        /// 
        /// Security Note:
        /// This endpoint does not require authentication and can be called publicly.
        /// It only validates the token without revealing sensitive information.
        /// </remarks>
        /// <param name="model">Access token to validate</param>
        /// <returns>Token validation result with status and explanation</returns>
        /// <response code="200">Validation completed successfully. Check 'isValid' field for result.</response>
        /// <response code="400">Invalid request format or missing token.</response>
        /// <response code="500">Internal server error during validation process.</response>
        [HttpPost("validate-access-token")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ValidateAccessToken([FromBody] ValidateAccessTokenModel model)
        {
            var isValid = await _authService.ValidateAccessTokenAsync(model.AccessToken);
            return Ok(new { 
                isValid = isValid,
                message = isValid ? "Access token is valid" : "Access token is invalid or revoked"
            });
        }

        /// <summary>
        /// Validate refresh token
        /// </summary>
        /// <remarks>
        /// Validates a refresh token's signature, expiration, and whitelist status.
        /// 
        /// Validation Checks:
        /// 1. Signature Verification: Ensures token wasn't tampered with
        /// 2. Expiration Check: Verifies token is still within validity period
        /// 3. Whitelist Status: Checks if token is in active whitelist
        /// 4. Format Validation: Ensures proper JWT structure
        /// 5. Claims Validation: Verifies issuer, audience, and other claims
        /// 
        /// Whitelist System:
        /// - Refresh tokens are stored in secure whitelist upon creation
        /// - Only whitelisted tokens can be used for token refresh
        /// - Tokens are removed from whitelist on logout or revocation
        /// - Provides additional security layer beyond signature validation
        /// 
        /// Use Cases:
        /// - Pre-validation before attempting token refresh
        /// - Client-side token health checking
        /// - Security monitoring and auditing
        /// - Debugging refresh token issues
        /// 
        /// Response Information:
        /// - isValid: Boolean indicating overall token validity
        /// - message: Human-readable explanation of validation result
        /// 
        /// Token States:
        /// - Valid: Token passes all validation checks and is whitelisted
        /// - Expired: Token signature is valid but expired
        /// - Not Whitelisted: Token is valid but not in active whitelist
        /// - Invalid: Token signature or format is incorrect
        /// 
        /// Security Note:
        /// This endpoint does not require authentication but should be used carefully
        /// as refresh tokens are more sensitive than access tokens.
        /// </remarks>
        /// <param name="model">Refresh token to validate</param>
        /// <returns>Token validation result with status and explanation</returns>
        /// <response code="200">Validation completed successfully. Check 'isValid' field for result.</response>
        /// <response code="400">Invalid request format or missing token.</response>
        /// <response code="500">Internal server error during validation process.</response>
        [HttpPost("validate-refresh-token")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
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
