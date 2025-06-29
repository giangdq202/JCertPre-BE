using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.Auth;
using JCertPreApplication.Application.Dtos.User;
using JCertPreApplication.Domain.Configuration;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JCertPreApplication.Application.Features.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IFirebaseService _firebaseService;
        private readonly IPasswordService _passwordService;
        private readonly JwtConfiguration _jwtConfig;

        public AuthService(IUserRepository userRepository, IRoleRepository roleRepository, IFirebaseService firebaseService, IPasswordService passwordService, IOptions<JwtConfiguration> jwtConfig)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
            _firebaseService = firebaseService ?? throw new ArgumentNullException(nameof(firebaseService));
            _passwordService = passwordService ?? throw new ArgumentNullException(nameof(passwordService));
            _jwtConfig = jwtConfig?.Value ?? throw new ArgumentNullException(nameof(jwtConfig));
        }

        public async Task<(string AccessToken, string RefreshToken, AppUserDto User)> LoginAsync(string email, string password)
        {
            var user = await _userRepository.GetFirstOrDefaultAsync(u => u.email == email);
            if (user == null || user.status != UserStatus.Active || !_passwordService.VerifyPassword(password, user.passwordHash))
            {
                return (null, null, null);
            }

            return GenerateTokensAndUserDto(user);
        }

        public async Task<(bool Succeeded, string AccessToken, string RefreshToken, AppUserDto User, string[] Errors)> RegisterAsync(RegisterModel model)
        {
            // Check if email already exists
            var existingUserByEmail = await _userRepository.GetFirstOrDefaultAsync(u => u.email == model.email);
            if (existingUserByEmail != null)
            {
                return (false, null, null, null, new[] { "Email already exists." });
            }

            // Phone is optional information, no need to check for uniqueness

            var hashedPassword = _passwordService.HashPassword(model.password);
            var defaultRole = await _roleRepository.GetByRoleNameAsync("STUDENT");
            if (defaultRole == null)
            {
                defaultRole = new Role { roleId = Guid.NewGuid(), roleName = "STUDENT", description = "Default role" };
                await _roleRepository.InsertAsync(defaultRole);
                await _roleRepository.SaveChangesAsync();
            }

            var user = new User
            {
                userId = Guid.NewGuid(),
                fullName = model.fullName,
                email = model.email,
                passwordHash = hashedPassword,
                phone = model.phone,
                avatarUrl = model.avatarUrl,
                credit = 0,
                createdAt = DateTime.UtcNow,
                lastLogin = DateTime.UtcNow,
                status = UserStatus.Active,
                roleId = defaultRole.roleId // Gán roleId trực tiếp
            };

            await _userRepository.InsertAsync(user);
            await _userRepository.SaveChangesAsync();

            var (accessToken, refreshToken, userDto) = GenerateTokensAndUserDto(user);
            return (true, accessToken, refreshToken, userDto, null);
        }

        public async Task<(string AccessToken, string RefreshToken, AppUserDto User)> RefreshTokenAsync(string refreshToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var refreshKey = Encoding.UTF8.GetBytes(_jwtConfig.RefreshSecretKey);

            try
            {
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
                    return (null, null, null);

                var userId = Guid.Parse(principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);
                if (userId == Guid.Empty)
                    return (null, null, null);

                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                    return (null, null, null);

                return GenerateTokensAndUserDto(user);
            }
            catch
            {
                return (null, null, null);
            }
        }

        public async Task<(string AccessToken, string RefreshToken, AppUserDto User)> FirebaseLoginAsync(string firebaseToken)
        {
            try
            {
                // Verify Firebase token and get user info
                var (email, name, picture) = await _firebaseService.GetUserInfoFromTokenAsync(firebaseToken);

                if (string.IsNullOrEmpty(email))
                {
                    return (null, null, null);
                }

                // Check if user exists in database
                var existingUser = await _userRepository.GetFirstOrDefaultAsync(u => u.email == email);

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

                return GenerateTokensAndUserDto(user);
            }
            catch (UnauthorizedAccessException)
            {
                return (null, null, null);
            }
            catch (Exception)
            {
                return (null, null, null);
            }
        }

        /// <summary>
        /// Generates JWT access token, refresh token, and user DTO for authenticated user.
        /// This method centralizes token generation logic to avoid code duplication.
        /// </summary>
        /// <param name="user">The authenticated user entity</param>
        /// <returns>Tuple containing access token, refresh token, and user DTO</returns>
        private (string AccessToken, string RefreshToken, AppUserDto UserDto) GenerateTokensAndUserDto(User user)
        {
            var defaultRole = user.Role?.roleName ?? "STUDENT";

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.fullName),
                new Claim(ClaimTypes.NameIdentifier, user.userId.ToString()),
                new Claim(ClaimTypes.Role, defaultRole)
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

            var userDto = new AppUserDto
            {
                Id = user.userId,
                fullName = user.fullName,
                phone = user.phone,
                email = user.email
            };

            return (accessTokenString, refreshTokenString, userDto);
        }

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
    }
}
