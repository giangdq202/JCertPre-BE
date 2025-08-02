using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.Auth;
using JCertPreApplication.Application.Dtos.User;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Domain.Configuration;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;

namespace JCertPreApplication.Application.Features.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IFirebaseService _firebaseService;
        private readonly IPasswordService _passwordService;
        private readonly ITokenCacheRepository _tokenCacheRepository;
        private readonly ICacheRepository _cacheRepository;
        private readonly IFileService _fileService;
        private readonly IMailService _mailService;
        private readonly JwtConfiguration _jwtConfig;
        private readonly FrontendConfiguration _frontendConfig;

        public AuthService(IUserRepository userRepository, IRoleRepository roleRepository, IFirebaseService firebaseService, IPasswordService passwordService, ITokenCacheRepository tokenCacheRepository, ICacheRepository cacheRepository, IFileService fileService, IMailService mailService, IOptions<JwtConfiguration> jwtConfig, IOptions<FrontendConfiguration> frontendConfig)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
            _firebaseService = firebaseService ?? throw new ArgumentNullException(nameof(firebaseService));
            _passwordService = passwordService ?? throw new ArgumentNullException(nameof(passwordService));
            _tokenCacheRepository = tokenCacheRepository ?? throw new ArgumentNullException(nameof(tokenCacheRepository));
            _cacheRepository = cacheRepository ?? throw new ArgumentNullException(nameof(cacheRepository));
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
            _jwtConfig = jwtConfig?.Value ?? throw new ArgumentNullException(nameof(jwtConfig));
            _frontendConfig = frontendConfig?.Value ?? throw new ArgumentNullException(nameof(frontendConfig));
        }

        public async Task<(string AccessToken, string RefreshToken, AuthUserDto User)> LoginAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailWithRoleAsync(email);
            
            if (user == null || !_passwordService.VerifyPassword(password, user.passwordHash))
            {
                throw ApiException.Unauthorized("INVALID_CREDENTIALS", "Invalid email or password.");
            }
            
            if (user.status != UserStatus.Active)
            {
                throw ApiException.Forbidden("ACCOUNT_INACTIVE", "Your account is inactive. Please contact support.");
            }

            var (accessToken, refreshToken, userDto) = GenerateTokensAndUserDto(user);
            
            // Add refresh token to whitelist
            await _tokenCacheRepository.AddRefreshTokenAsync(user.userId, refreshToken);

            return (accessToken, refreshToken, userDto);
        }

        public async Task<(string AccessToken, string RefreshToken, AuthUserDto User)> RegisterAsync(RegisterModel model)
        {
            // Check if email already exists
            var existingUserByEmail = await _userRepository.GetByEmailWithRoleAsync(model.Email);
            if (existingUserByEmail != null)
            {
                throw ApiException.BadRequest("EMAIL_ALREADY_EXISTS", $"User with email '{model.Email}' already exists.");
            }

            // Phone is optional information, no need to check for uniqueness

            var hashedPassword = _passwordService.HashPassword(model.Password);
            var defaultRole = await _roleRepository.GetByRoleNameAsync("STUDENT");
            if (defaultRole == null)
            {
                throw ApiException.InternalServerError("DEFAULT_ROLE_NOT_FOUND", "Default role STUDENT not found in the system.");
            }

            // Generate userId first to use for avatar filename
            var userId = Guid.NewGuid();
            string? avatarUrl = null;

            // Handle avatar file upload if provided
            if (model.AvatarFile != null)
            {
                // Create a custom FormFile with userId as filename
                var customFormFile = CreateCustomFormFile(model.AvatarFile, userId.ToString());

                // Upload avatar to file service
                var uploadResult = await _fileService.UploadImageAsync(customFormFile);
                avatarUrl = uploadResult.SecureUrl.ToString();
            }

            var user = new User
            {
                userId = userId,
                fullName = model.FullName,
                email = model.Email,
                passwordHash = hashedPassword,
                phone = model.Phone,
                avatarUrl = avatarUrl,
                credit = 0,
                createdAt = DateTime.UtcNow,
                lastLogin = DateTime.UtcNow,
                status = UserStatus.Active,
                roleId = defaultRole.roleId // Gán roleId trực tiếp
            };

            await _userRepository.InsertAsync(user);
            await _userRepository.SaveChangesAsync();

            // Send welcome email
            await SendWelcomeEmailAsync(user.email, user.fullName);

            var (accessToken, refreshToken, userDto) = GenerateTokensAndUserDto(user);
            
            // Add refresh token to whitelist
            await _tokenCacheRepository.AddRefreshTokenAsync(user.userId, refreshToken);

            return (accessToken, refreshToken, userDto);
        }

        public async Task<(string AccessToken, string RefreshToken, AuthUserDto User)> RefreshTokenAsync(string accessToken, string refreshToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            
            try
            {
                // Step 1: Validate the old access token and extract information
                var oldAccessTokenJson = tokenHandler.ReadJwtToken(accessToken);
                var oldJti = oldAccessTokenJson.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
                if (string.IsNullOrEmpty(oldJti))
                {
                    throw ApiException.BadRequest("INVALID_ACCESS_TOKEN", "Access token does not contain JTI claim.");
                }

                var accessTokenUserId = oldAccessTokenJson.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(accessTokenUserId) || !Guid.TryParse(accessTokenUserId, out var accessTokenUserGuid))
                {
                    throw ApiException.BadRequest("INVALID_ACCESS_TOKEN", "Invalid user ID in access token.");
                }

                // Step 2: Validate the refresh token
                var refreshKey = Encoding.UTF8.GetBytes(_jwtConfig.RefreshSecretKey);
                var principal = tokenHandler.ValidateToken(
                    refreshToken,
                    new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(refreshKey),
                        ValidateIssuer = true,
                        ValidIssuer = _jwtConfig.Issuer,
                        ValidateAudience = true,
                        ValidAudience = _jwtConfig.Audience,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    },
                    out SecurityToken validatedToken
                );

                if (principal.FindFirst("type")?.Value != "refresh")
                {
                    throw ApiException.Unauthorized("INVALID_REFRESH_TOKEN", "Invalid refresh token type.");
                }

                var refreshTokenUserId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(refreshTokenUserId) || !Guid.TryParse(refreshTokenUserId, out var refreshTokenUserGuid))
                {
                    throw ApiException.Unauthorized("INVALID_REFRESH_TOKEN", "Invalid user ID in refresh token.");
                }

                // Step 3: Ensure both tokens belong to the same user
                if (accessTokenUserGuid != refreshTokenUserGuid)
                {
                    throw ApiException.BadRequest("TOKEN_MISMATCH", "Access token and refresh token belong to different users.");
                }

                var userId = refreshTokenUserGuid;

                // Step 4: Check if refresh token is in whitelist (Refresh Token Rotation)
                var isRefreshTokenValid = await _tokenCacheRepository.IsRefreshTokenValidAsync(userId, refreshToken);
                if (!isRefreshTokenValid)
                {
                    throw ApiException.Unauthorized("INVALID_REFRESH_TOKEN", "Refresh token not found in whitelist or already used.");
                }

                // Step 5: Immediately revoke the old access token (Critical security step)
                var oldAccessTokenExpiration = oldAccessTokenJson.ValidTo;
                var remainingLifetime = oldAccessTokenExpiration - DateTime.UtcNow;
                if (remainingLifetime > TimeSpan.Zero)
                {
                    await _tokenCacheRepository.RevokeAccessTokenAsync(oldJti, remainingLifetime);
                }

                // Step 6: Remove old refresh token from whitelist (Refresh Token Rotation)
                await _tokenCacheRepository.RemoveRefreshTokenAsync(userId, refreshToken);

                // Step 7: Get user and generate new tokens
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    throw ApiException.Unauthorized("USER_NOT_FOUND", "User not found.");
                }

                // Step 8: Generate new tokens
                var (newAccessToken, newRefreshToken, userDto) = GenerateTokensAndUserDto(user);

                // Step 9: Add new refresh token to whitelist
                await _tokenCacheRepository.AddRefreshTokenAsync(userId, newRefreshToken);

                return (newAccessToken, newRefreshToken, userDto);
            }
            catch (ApiException)
            {
                // Re-throw our custom exceptions
                throw;
            }
            catch (SecurityTokenException)
            {
                throw ApiException.Unauthorized("INVALID_ACCESS_TOKEN", "Invalid or malformed access token.");
            }
            catch (Exception)
            {
                // Any other exception during token validation means invalid token
                throw ApiException.Unauthorized("TOKEN_REFRESH_ERROR", "An error occurred during token refresh.");
            }
        }

        public async Task<(string AccessToken, string RefreshToken, AuthUserDto User)> FirebaseLoginAsync(string firebaseToken)
        {
            try
            {
                // Verify Firebase token and get user info
                var (email, name, picture) = await _firebaseService.GetUserInfoFromTokenAsync(firebaseToken);

                if (string.IsNullOrEmpty(email))
                {
                    throw ApiException.Unauthorized("INVALID_FIREBASE_TOKEN", "Invalid Firebase token or email not found.");
                }

                // Check if user exists in database
                var existingUser = await _userRepository.GetByEmailWithRoleAsync(email);

                User user;
                if (existingUser == null)
                {
                    // User doesn't exist, create new user (onboarding)
                    var defaultRole = await _roleRepository.GetByRoleNameAsync("STUDENT");
                    if (defaultRole == null)
                    {
                        defaultRole = new Role { roleId = Guid.NewGuid(), roleName = "STUDENT", description = "Default role" };
                        await _roleRepository.InsertAsync(defaultRole);
                        await _roleRepository.SaveChangesAsync();
                    }

                    user = new User
                    {
                        userId = Guid.NewGuid(),
                        fullName = name,
                        email = email,
                        passwordHash = string.Empty, // Firebase users don't have local password
                        avatarUrl = picture,
                        credit = 0,
                        createdAt = DateTime.UtcNow,
                        lastLogin = DateTime.UtcNow,
                        status = UserStatus.Active,
                        roleId = defaultRole.roleId
                    };

                    await _userRepository.InsertAsync(user);
                    await _userRepository.SaveChangesAsync();

                    // Send welcome email for new Firebase user
                    await SendWelcomeEmailAsync(user.email, user.fullName);
                }
                else
                {
                    // User exists, update last login and avatar if needed
                    user = existingUser;
                    user.lastLogin = DateTime.UtcNow;
                    
                    // Update avatar if Firebase has a newer one
                    if (!string.IsNullOrEmpty(picture) && user.avatarUrl != picture)
                    {
                        user.avatarUrl = picture;
                    }

                    await _userRepository.UpdateAsync(user);
                    await _userRepository.SaveChangesAsync();
                }

                var (accessToken, refreshToken, userDto) = GenerateTokensAndUserDto(user);
                
                // Add refresh token to whitelist
                await _tokenCacheRepository.AddRefreshTokenAsync(user.userId, refreshToken);

                return (accessToken, refreshToken, userDto);
            }
            catch (ApiException)
            {
                // Re-throw our custom exceptions
                throw;
            }
            catch (UnauthorizedAccessException)
            {
                throw ApiException.Unauthorized("FIREBASE_AUTH_FAILED", "Firebase authentication failed.");
            }
            catch (Exception)
            {
                throw ApiException.InternalServerError("FIREBASE_LOGIN_ERROR", "An error occurred during Firebase login.");
            }
        }

        /// <summary>
        /// Generates JWT access token, refresh token, and user DTO for authenticated user.
        /// This method centralizes token generation logic to avoid code duplication.
        /// </summary>
        /// <param name="user">The authenticated user entity</param>
        /// <returns>Tuple containing access token, refresh token, and user DTO</returns>
        private (string AccessToken, string RefreshToken, AuthUserDto UserDto) GenerateTokensAndUserDto(User user)
        {
            var defaultRole = user.Role?.roleName ?? "STUDENT";
            var jti = Guid.NewGuid().ToString(); // JWT ID for token revocation

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.fullName),
                new Claim(ClaimTypes.NameIdentifier, user.userId.ToString()),
                new Claim(ClaimTypes.Role, defaultRole),
                new Claim(JwtRegisteredClaimNames.Jti, jti), // JWT ID claim
                new Claim(JwtRegisteredClaimNames.Iat, 
                    new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(), 
                    ClaimValueTypes.Integer64) // Issued At claim
            };

            var accessKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.SecretKey));
            var accessCreds = new SigningCredentials(accessKey, SecurityAlgorithms.HmacSha256);
            var accessToken = new JwtSecurityToken(
                issuer: _jwtConfig.Issuer,
                audience: _jwtConfig.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtConfig.ExpiryInMinutes),
                signingCredentials: accessCreds
            );
            var accessTokenString = new JwtSecurityTokenHandler().WriteToken(accessToken);

            var refreshTokenString = GenerateRefreshTokenAsJwt(user.userId);

            var userDto = new AuthUserDto
            {
                Id = user.userId,
                FullName = user.fullName,
                Email = user.email,
                Phone = user.phone,
                AvatarUrl = user.avatarUrl,
                Credit = user.credit,
                RoleName = defaultRole
            };

            return (accessTokenString, refreshTokenString, userDto);
        }

        /// <summary>
        /// Generates a refresh token as JWT with minimal claims for security
        /// </summary>
        /// <param name="userId">The user ID to include in the token</param>
        /// <returns>Refresh token as JWT string</returns>
        private string GenerateRefreshTokenAsJwt(Guid userId)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim("type", "refresh")
            };

            var refreshKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.RefreshSecretKey));
            var refreshCreds = new SigningCredentials(refreshKey, SecurityAlgorithms.HmacSha256);

            var refreshToken = new JwtSecurityToken(
                issuer: _jwtConfig.Issuer,
                audience: _jwtConfig.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: refreshCreds
            );

            return new JwtSecurityTokenHandler().WriteToken(refreshToken);
        }

        public async Task LogoutAsync(string accessToken, string refreshToken)
        {
            try
            {
                // Parse access token to extract JTI and expiration
                var tokenHandler = new JwtSecurityTokenHandler();
                var jsonToken = tokenHandler.ReadJwtToken(accessToken);

                var jti = jsonToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
                if (string.IsNullOrEmpty(jti))
                {
                    throw ApiException.BadRequest("INVALID_ACCESS_TOKEN", "Access token does not contain JTI claim.");
                }

                var userIdClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    throw ApiException.BadRequest("INVALID_ACCESS_TOKEN", "Invalid user ID in access token.");
                }

                // Calculate remaining lifetime of access token
                var expiration = jsonToken.ValidTo;
                var remainingLifetime = expiration - DateTime.UtcNow;

                // Revoke access token (add to blacklist)
                if (remainingLifetime > TimeSpan.Zero)
                {
                    await _tokenCacheRepository.RevokeAccessTokenAsync(jti, remainingLifetime);
                }

                // Remove refresh token from whitelist
                if (!string.IsNullOrEmpty(refreshToken))
                {
                    await _tokenCacheRepository.RemoveRefreshTokenAsync(userId, refreshToken);
                }
            }
            catch (ApiException)
            {
                // Re-throw our custom exceptions
                throw;
            }
            catch (Exception)
            {
                throw ApiException.InternalServerError("LOGOUT_ERROR", "An error occurred during logout.");
            }
        }

        public async Task<bool> ValidateAccessTokenAsync(string accessToken)
        {
            try
            {
                if (string.IsNullOrEmpty(accessToken))
                    return false;

                // Parse token to validate structure and get claims
                var tokenHandler = new JwtSecurityTokenHandler();
                var jsonToken = tokenHandler.ReadJwtToken(accessToken);

                // Check if token is expired
                if (jsonToken.ValidTo < DateTime.UtcNow)
                    return false;

                // Get JTI to check if token is revoked
                var jti = jsonToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
                if (string.IsNullOrEmpty(jti))
                    return false;

                // Check if token is in blacklist (revoked)
                var isRevoked = await _tokenCacheRepository.IsAccessTokenRevokedAsync(jti);
                return !isRevoked;
            }
            catch (Exception)
            {
                // Any exception means invalid token
                return false;
            }
        }

        public async Task<bool> ValidateRefreshTokenAsync(string refreshToken)
        {
            try
            {
                if (string.IsNullOrEmpty(refreshToken))
                    return false;

                // Validate JWT structure and signature
                var tokenHandler = new JwtSecurityTokenHandler();
                var refreshKey = Encoding.UTF8.GetBytes(_jwtConfig.RefreshSecretKey);

                var principal = tokenHandler.ValidateToken(
                    refreshToken,
                    new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(refreshKey),
                        ValidateIssuer = true,
                        ValidIssuer = _jwtConfig.Issuer,
                        ValidateAudience = true,
                        ValidAudience = _jwtConfig.Audience,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    },
                    out SecurityToken validatedToken
                );

                // Check token type
                if (principal.FindFirst("type")?.Value != "refresh")
                    return false;

                // Extract userId and check if token is in whitelist
                var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                    return false;

                // Check if refresh token is in whitelist
                return await _tokenCacheRepository.IsRefreshTokenValidAsync(userId, refreshToken);
            }
            catch (Exception)
            {
                // Any exception means invalid token
                return false;
            }
        }

        #region Private Helper Methods

        private static IFormFile CreateCustomFormFile(IFormFile originalFile, string customFileName)
        {
            // Get the file extension from original file
            var extension = Path.GetExtension(originalFile.FileName);
            var newFileName = customFileName + extension;

            return new CustomFormFile(originalFile, newFileName);
        }

        private async Task SendWelcomeEmailAsync(string email, string fullName)
        {
            try
            {
                var templateData = new
                {
                    Name = fullName,
                    Email = email,
                    PlatformUrl = _frontendConfig.BaseUrl
                };

                await _mailService.SendTemplateEmailAsync(email, "welcome", templateData);
            }
            catch (Exception ex)
            {
                // Log error but don't throw - we don't want email failures to break registration
                // You can inject ILogger<AuthService> if needed for better logging
                Console.WriteLine($"Failed to send welcome email to {email}: {ex.Message}");
            }
        }

        #endregion

        #region Password Reset Operations

        public async Task<string> ForgotPasswordAsync(string email, string? ipAddress = null)
        {
            try
            {
                // Validate email format
                if (string.IsNullOrWhiteSpace(email))
                    throw ApiException.BadRequest("INVALID_EMAIL", "Email address is required.");

                // Check if user with this email exists
                var user = await _userRepository.GetByEmailAsync(email);
                
                // For security reasons, always return success message even if email doesn't exist
                // This prevents email enumeration attacks
                if (user == null)
                {
                    // Still return success to prevent email enumeration
                    return "If the email address exists in our system, you will receive password reset instructions.";
                }

                // Check if user account is active
                if (user.status != UserStatus.Active)
                    return "If the email address exists in our system, you will receive password reset instructions.";

                // Generate secure reset token
                var resetToken = GenerateSecureToken();
                
                // Create reset data to store in Redis
                var resetData = new PasswordResetTokenData
                {
                    UserId = user.userId,
                    Email = user.email,
                    CreatedAt = DateTime.UtcNow,
                    IpAddress = ipAddress,
                    IsUsed = false
                };

                // Store token in Redis with 15-minute TTL
                var cacheKey = $"password-reset:{resetToken}";
                await _cacheRepository.SetAsync(cacheKey, resetData, TimeSpan.FromMinutes(15));

                // Send reset email with token
                var resetLink = $"{_frontendConfig.ResetPasswordUrl}?token={resetToken}";
                await _mailService.SendTemplateEmailAsync(user.email, "password-reset", new 
                { 
                    Name = user.fullName, 
                    ResetLink = resetLink,
                    ExpiryMinutes = 15
                });

                return "Nếu địa chỉ email tồn tại trong hệ thống, bạn sẽ nhận được hướng dẫn đặt lại mật khẩu.";
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("FORGOT_PASSWORD_ERROR", 
                    "Đã xảy ra lỗi khi xử lý yêu cầu đặt lại mật khẩu của bạn.");
            }
        }

        public async Task<string> ResetPasswordAsync(string token, string newPassword, string? ipAddress = null)
        {
            try
            {
                // Validate inputs
                if (string.IsNullOrWhiteSpace(token))
                    throw ApiException.BadRequest("INVALID_TOKEN", "Token đặt lại mật khẩu là bắt buộc.");

                if (string.IsNullOrWhiteSpace(newPassword))
                    throw ApiException.BadRequest("INVALID_PASSWORD", "Mật khẩu mới là bắt buộc.");

                if (newPassword.Length < 6)
                    throw ApiException.BadRequest("WEAK_PASSWORD", "Mật khẩu phải có ít nhất 6 ký tự.");

                // Retrieve reset data from Redis
                var cacheKey = $"password-reset:{token}";
                var resetData = await _cacheRepository.GetAsync<PasswordResetTokenData>(cacheKey);

                if (resetData == null)
                    throw ApiException.BadRequest("INVALID_OR_EXPIRED_TOKEN", 
                        "Token đặt lại mật khẩu không hợp lệ hoặc đã hết hạn. Vui lòng yêu cầu đặt lại mật khẩu mới.");

                // Check if token has already been used
                if (resetData.IsUsed)
                    throw ApiException.BadRequest("TOKEN_ALREADY_USED", 
                        "Token đặt lại này đã được sử dụng. Vui lòng yêu cầu đặt lại mật khẩu mới.");

                // Get user from database
                var user = await _userRepository.GetByIdAsync(resetData.UserId);
                if (user == null)
                    throw ApiException.NotFound("USER_NOT_FOUND", "User account not found.");

                // Check if user account is still active
                if (user.status != UserStatus.Active)
                    throw ApiException.BadRequest("ACCOUNT_INACTIVE", "User account is not active.");

                // Hash the new password
                var hashedPassword = _passwordService.HashPassword(newPassword);

                // Update user password
                user.passwordHash = hashedPassword;
                user.lastLogin = DateTime.UtcNow; // Update last login as security measure

                await _userRepository.UpdateAsync(user);
                await _userRepository.SaveChangesAsync();

                // Mark token as used and update in Redis (keep for audit purposes until TTL expires)
                resetData.IsUsed = true;
                resetData.UsedAt = DateTime.UtcNow;
                await _cacheRepository.SetAsync(cacheKey, resetData, TimeSpan.FromMinutes(15)); // Keep until original TTL

                // Revoke all existing refresh tokens for security
                await _tokenCacheRepository.RemoveAllRefreshTokensAsync(user.userId);

                // Optional: Send confirmation email
                try
                {
                    await _mailService.SendTemplateEmailAsync(user.email, "password-changed", new 
                    { 
                        Name = user.fullName,
                        ChangeTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss UTC"),
                        IpAddress = ipAddress ?? "Unknown",
                        UserAgent = "N/A", // Có thể thêm từ HttpContext nếu cần
                        LoginUrl = _frontendConfig.LoginUrl
                    });
                }
                catch (Exception)
                {
                    // Don't fail the operation if email sending fails
                }

                return "Mật khẩu của bạn đã được đặt lại thành công. Tất cả phiên đăng nhập hiện tại đã được đăng xuất để đảm bảo bảo mật.";
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("RESET_PASSWORD_ERROR", 
                    "An error occurred while resetting your password.");
            }
        }

        public async Task<bool> ValidateResetTokenAsync(string token)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token))
                    return false;

                var cacheKey = $"password-reset:{token}";
                var resetData = await _cacheRepository.GetAsync<PasswordResetTokenData>(cacheKey);

                // Token is valid if it exists in Redis and hasn't been used
                return resetData != null && !resetData.IsUsed;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Generates a cryptographically secure token for password reset
        /// </summary>
        private static string GenerateSecureToken()
        {
            using var rng = RandomNumberGenerator.Create();
            var tokenBytes = new byte[32]; // 256 bits
            rng.GetBytes(tokenBytes);
            return Convert.ToBase64String(tokenBytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
        }

        #endregion
    }

    /// <summary>
    /// Custom IFormFile implementation to override filename while preserving original file content
    /// </summary>
    internal class CustomFormFile : IFormFile
    {
        private readonly IFormFile _originalFile;
        private readonly string _customFileName;

        public CustomFormFile(IFormFile originalFile, string customFileName)
        {
            _originalFile = originalFile;
            _customFileName = customFileName;
        }

        public string ContentType => _originalFile.ContentType;
        public string ContentDisposition => _originalFile.ContentDisposition;
        public IHeaderDictionary Headers => _originalFile.Headers;
        public long Length => _originalFile.Length;
        public string Name => _originalFile.Name;
        public string FileName => _customFileName; // This is the overridden filename

        public void CopyTo(Stream target) => _originalFile.CopyTo(target);
        public Task CopyToAsync(Stream target, CancellationToken cancellationToken = default) =>
            _originalFile.CopyToAsync(target, cancellationToken);
        public Stream OpenReadStream() => _originalFile.OpenReadStream();
    }
}
