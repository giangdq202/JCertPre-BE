using JCertPreApplication.Application.Configuration;
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
        private readonly JwtConfiguration _jwtConfig;

        public AuthService(IUserRepository userRepository, IRoleRepository roleRepository, IOptions<JwtConfiguration> jwtConfig)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
            _jwtConfig = jwtConfig?.Value ?? throw new ArgumentNullException(nameof(jwtConfig));
        }

        public async Task<(string AccessToken, string RefreshToken, AppUserDto User)> LoginAsync(string username, string password)
        {
            var user = await _userRepository.GetFirstOrDefaultAsync(u => u.email == username || 
                                                                     (!string.IsNullOrEmpty(u.phone) && u.phone == username));
            if (user == null || user.status != UserStatus.Active || !VerifyPassword(password, user.passwordHash))
            {
                return (null, null, null);
            }

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

        public async Task<(bool Succeeded, string AccessToken, string RefreshToken, AppUserDto User, string[] Errors)> RegisterAsync(RegisterModel model)
        {
            // Check if email already exists
            var existingUserByEmail = await _userRepository.GetFirstOrDefaultAsync(u => u.email == model.email);
            if (existingUserByEmail != null)
            {
                return (false, null, null, null, new[] { "Email already exists." });
            }

            // Check if phone already exists (only if phone is provided)
            if (!string.IsNullOrWhiteSpace(model.phone))
            {
                var existingUserByPhone = await _userRepository.GetFirstOrDefaultAsync(u => u.phone == model.phone);
                if (existingUserByPhone != null)
                {
                    return (false, null, null, null, new[] { "Phone number already exists." });
                }
            }

            var hashedPassword = HashPassword(model.passwordHash);
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

            var claims = new[]
            {
            new Claim(ClaimTypes.Name, user.fullName),
            new Claim(ClaimTypes.NameIdentifier, user.userId.ToString()),
            new Claim(ClaimTypes.Role, "STUDENT")
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

            return (true, accessTokenString, refreshTokenString, userDto, null);
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

                var defaultRole = user.Role?.roleName ?? "STUDENT";

                var claims = new[]
                {
                new Claim(ClaimTypes.Name, user.fullName),
                new Claim(ClaimTypes.NameIdentifier, user.userId.ToString()),
                new Claim(ClaimTypes.Role, defaultRole)
            };

                var accessKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.SecretKey));
                var creds = new SigningCredentials(accessKey, SecurityAlgorithms.HmacSha256);

                var newAccessToken = new JwtSecurityToken(
                    issuer: _jwtConfig.Issuer,
                    audience: _jwtConfig.Audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(_jwtConfig.ExpiryInMinutes),
                    signingCredentials: creds
                );

                var newAccessTokenString = new JwtSecurityTokenHandler().WriteToken(newAccessToken);
                var newRefreshTokenString = GenerateRefreshTokenAsJwt(user.userId);

                var userDto = new AppUserDto
                {
                    Id = user.userId,
                    fullName = user.fullName,
                    phone = user.phone,
                    email = user.email
                };

                return (newAccessTokenString, newRefreshTokenString, userDto);
            }
            catch
            {
                return (null, null, null);
            }
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

        private string HashPassword(string password)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(password));
        }

        private bool VerifyPassword(string inputPassword, string passwordHash)
        {
            var hashedInput = Convert.ToBase64String(Encoding.UTF8.GetBytes(inputPassword));
            return hashedInput == passwordHash;
        }
    }
}
