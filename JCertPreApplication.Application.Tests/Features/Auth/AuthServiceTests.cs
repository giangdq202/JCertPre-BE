using System.Linq.Expressions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Application.Features.Auth;
using JCertPreApplication.Application.Dtos.User;
using JCertPreApplication.Domain.Configuration;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;
using JCertPreApplication.Application.Dtos.Auth;

namespace JCertPreApplication.Application.Tests.Features.Auth
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IRoleRepository> _roleRepositoryMock;
        private readonly Mock<IFirebaseService> _firebaseServiceMock;
        private readonly Mock<IPasswordService> _passwordServiceMock;
        private readonly Mock<ITokenCacheRepository> _tokenCacheRepositoryMock;
        private readonly IOptions<JwtConfiguration> _jwtOptions;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _roleRepositoryMock = new Mock<IRoleRepository>();
            _firebaseServiceMock = new Mock<IFirebaseService>();
            _passwordServiceMock = new Mock<IPasswordService>();
            _tokenCacheRepositoryMock = new Mock<ITokenCacheRepository>();

            _jwtOptions = Options.Create(new JwtConfiguration
            {
                SecretKey = new string('A', 32), // simple 32-char secret
                RefreshSecretKey = new string('B', 32),
                Issuer = "TestIssuer",
                Audience = "TestAudience",
                ExpiryInMinutes = 60
            });

            _authService = new AuthService(
                _userRepositoryMock.Object,
                _roleRepositoryMock.Object,
                _firebaseServiceMock.Object,
                _passwordServiceMock.Object,
                _tokenCacheRepositoryMock.Object,
                _jwtOptions);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnTokens_WhenCredentialsAreValid()
        {
            // Arrange
            var email = "user@example.com";
            var password = "Password123";
            var user = CreateTestUser(email, UserStatus.Active);

            _userRepositoryMock
                .Setup(repo => repo.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<string?>()))
                .ReturnsAsync(user);

            _passwordServiceMock
                .Setup(ps => ps.VerifyPassword(password, user.passwordHash))
                .Returns(true);

            _tokenCacheRepositoryMock
                .Setup(tc => tc.AddRefreshTokenAsync(user.userId, It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var (accessToken, refreshToken, userDto) = await _authService.LoginAsync(email, password);

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(accessToken));
            Assert.False(string.IsNullOrWhiteSpace(refreshToken));
            Assert.NotNull(userDto);
            Assert.IsType<AppUserDto>(userDto);
            Assert.Equal(user.userId, userDto.Id);

            _tokenCacheRepositoryMock.Verify(tc => tc.AddRefreshTokenAsync(user.userId, It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task LoginAsync_ShouldThrowUnauthorized_WhenPasswordInvalid()
        {
            // Arrange
            var email = "user@example.com";
            var password = "BadPassword";
            var user = CreateTestUser(email, UserStatus.Active);

            _userRepositoryMock
                .Setup(repo => repo.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<string?>()))
                .ReturnsAsync(user);

            _passwordServiceMock
                .Setup(ps => ps.VerifyPassword(password, user.passwordHash))
                .Returns(false);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ApiException>(() => _authService.LoginAsync(email, password));
            Assert.Equal("INVALID_CREDENTIALS", ex.ErrorCode);
        }

        [Fact]
        public async Task LoginAsync_ShouldThrowForbidden_WhenAccountInactive()
        {
            // Arrange
            var email = "user@example.com";
            var password = "Password123";
            var user = CreateTestUser(email, UserStatus.Inactive);

            _userRepositoryMock
                .Setup(repo => repo.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<string?>()))
                .ReturnsAsync(user);

            _passwordServiceMock
                .Setup(ps => ps.VerifyPassword(password, user.passwordHash))
                .Returns(true);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ApiException>(() => _authService.LoginAsync(email, password));
            Assert.Equal("ACCOUNT_INACTIVE", ex.ErrorCode);
        }

        [Fact]
        public async Task RegisterAsync_ShouldCreateUser_WhenDataValid()
        {
            // Arrange
            var registerModel = new RegisterModel
            {
                Email = "newuser@example.com",
                Password = "Password123",
                FullName = "New User"
            };

            _userRepositoryMock
                .Setup(repo => repo.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<string?>()))
                .ReturnsAsync((User?)null); // Email not yet taken

            _passwordServiceMock
                .Setup(ps => ps.HashPassword(registerModel.Password))
                .Returns("hashedPassword");

            var defaultRole = new Role { roleId = Guid.NewGuid(), roleName = "STUDENT" };
            _roleRepositoryMock
                .Setup(r => r.GetByRoleNameAsync("STUDENT"))
                .ReturnsAsync(defaultRole);

            _userRepositoryMock
                .Setup(repo => repo.InsertAsync(It.IsAny<User>()))
                .ReturnsAsync(new User { userId = Guid.NewGuid() });

            _userRepositoryMock
                .Setup(repo => repo.SaveChangesAsync())
                .ReturnsAsync(1);

            _tokenCacheRepositoryMock
                .Setup(tc => tc.AddRefreshTokenAsync(It.IsAny<Guid>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var (accessToken, refreshToken, userDto) = await _authService.RegisterAsync(registerModel);

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(accessToken));
            Assert.False(string.IsNullOrWhiteSpace(refreshToken));
            Assert.NotNull(userDto);
            Assert.Equal(registerModel.Email, userDto.email);

            _userRepositoryMock.Verify(repo => repo.InsertAsync(It.IsAny<User>()), Times.Once);
            _tokenCacheRepositoryMock.Verify(tc => tc.AddRefreshTokenAsync(It.IsAny<Guid>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_ShouldThrowBadRequest_WhenEmailAlreadyExists()
        {
            // Arrange
            var registerModel = new RegisterModel
            {
                Email = "existing@example.com",
                Password = "Password123",
                FullName = "Existing User"
            };

            var existingUser = CreateTestUser(registerModel.Email, UserStatus.Active);
            _userRepositoryMock
                .Setup(repo => repo.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<string?>()))
                .ReturnsAsync(existingUser);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ApiException>(() => _authService.RegisterAsync(registerModel));
            Assert.Equal("EMAIL_ALREADY_EXISTS", ex.ErrorCode);
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldReturnNewTokens_WhenTokensValid()
        {
            // Arrange (login first to obtain a valid pair of tokens)
            var user = CreateTestUser("refresher@example.com", UserStatus.Active);

            _userRepositoryMock
                .Setup(repo => repo.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<string?>()))
                .ReturnsAsync(user);
            _passwordServiceMock.Setup(ps => ps.VerifyPassword(It.IsAny<string>(), user.passwordHash)).Returns(true);
            _tokenCacheRepositoryMock.Setup(tc => tc.AddRefreshTokenAsync(user.userId, It.IsAny<string>())).Returns(Task.CompletedTask);

            var (accessToken, refreshToken, _) = await _authService.LoginAsync(user.email, "Password123");

            // Add a small delay to ensure different token timestamps
            await Task.Delay(1000);

            // Now set up mocks for refresh flow
            _tokenCacheRepositoryMock.Reset();
            _tokenCacheRepositoryMock.Setup(tc => tc.IsRefreshTokenValidAsync(user.userId, refreshToken)).ReturnsAsync(true);
            _tokenCacheRepositoryMock.Setup(tc => tc.RevokeAccessTokenAsync(It.IsAny<string>(), It.IsAny<TimeSpan>())).Returns(Task.CompletedTask);
            _tokenCacheRepositoryMock.Setup(tc => tc.RemoveRefreshTokenAsync(user.userId, refreshToken)).Returns(Task.CompletedTask);
            _tokenCacheRepositoryMock.Setup(tc => tc.AddRefreshTokenAsync(user.userId, It.IsAny<string>())).Returns(Task.CompletedTask);
            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(user.userId)).ReturnsAsync(user);

            // Act
            var (newAccess, newRefresh, userDto) = await _authService.RefreshTokenAsync(accessToken, refreshToken);

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(newAccess));
            Assert.False(string.IsNullOrWhiteSpace(newRefresh));
            Assert.NotEqual(accessToken, newAccess);
            Assert.NotEqual(refreshToken, newRefresh);
            Assert.Equal(user.userId, userDto.Id);

            _tokenCacheRepositoryMock.Verify(tc => tc.IsRefreshTokenValidAsync(user.userId, refreshToken), Times.Once);
            _tokenCacheRepositoryMock.Verify(tc => tc.AddRefreshTokenAsync(user.userId, It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldThrow_WhenRefreshTokenInvalid()
        {
            // Arrange
            var user = CreateTestUser("invalidrefresh@example.com", UserStatus.Active);
            _userRepositoryMock
                .Setup(repo => repo.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<string?>()))
                .ReturnsAsync(user);
            _passwordServiceMock.Setup(ps => ps.VerifyPassword(It.IsAny<string>(), user.passwordHash)).Returns(true);
            _tokenCacheRepositoryMock.Setup(tc => tc.AddRefreshTokenAsync(user.userId, It.IsAny<string>())).Returns(Task.CompletedTask);

            var (accessToken, refreshToken, _) = await _authService.LoginAsync(user.email, "Password123");

            // Set up invalid whitelist check
            _tokenCacheRepositoryMock.Reset();
            _tokenCacheRepositoryMock.Setup(tc => tc.IsRefreshTokenValidAsync(user.userId, refreshToken)).ReturnsAsync(false);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ApiException>(() => _authService.RefreshTokenAsync(accessToken, refreshToken));
            Assert.Equal("INVALID_REFRESH_TOKEN", ex.ErrorCode);
        }

        [Fact]
        public async Task ValidateAccessTokenAsync_ShouldReturnTrue_WhenTokenValid()
        {
            // Arrange
            var user = CreateTestUser("validate@example.com", UserStatus.Active);
            _userRepositoryMock.Setup(repo => repo.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<string?>())).ReturnsAsync(user);
            _passwordServiceMock.Setup(ps => ps.VerifyPassword(It.IsAny<string>(), user.passwordHash)).Returns(true);
            _tokenCacheRepositoryMock.Setup(tc => tc.AddRefreshTokenAsync(user.userId, It.IsAny<string>())).Returns(Task.CompletedTask);

            var (accessToken, _, _) = await _authService.LoginAsync(user.email, "Password123");

            _tokenCacheRepositoryMock.Reset();
            _tokenCacheRepositoryMock.Setup(tc => tc.IsAccessTokenRevokedAsync(It.IsAny<string>())).ReturnsAsync(false);

            // Act
            var isValid = await _authService.ValidateAccessTokenAsync(accessToken);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public async Task ValidateAccessTokenAsync_ShouldReturnFalse_WhenTokenRevoked()
        {
            // Arrange (generate a token)
            var user = CreateTestUser("revoked@example.com", UserStatus.Active);
            _userRepositoryMock.Setup(repo => repo.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<string?>())).ReturnsAsync(user);
            _passwordServiceMock.Setup(ps => ps.VerifyPassword(It.IsAny<string>(), user.passwordHash)).Returns(true);
            _tokenCacheRepositoryMock.Setup(tc => tc.AddRefreshTokenAsync(user.userId, It.IsAny<string>())).Returns(Task.CompletedTask);

            var (accessToken, _, _) = await _authService.LoginAsync(user.email, "Password123");

            // Token has been revoked
            _tokenCacheRepositoryMock.Reset();
            _tokenCacheRepositoryMock.Setup(tc => tc.IsAccessTokenRevokedAsync(It.IsAny<string>())).ReturnsAsync(true);

            // Act
            var isValid = await _authService.ValidateAccessTokenAsync(accessToken);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public async Task ValidateRefreshTokenAsync_ShouldReturnTrue_WhenTokenWhitelisted()
        {
            // Arrange
            var user = CreateTestUser("refreshvalid@example.com", UserStatus.Active);
            _userRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<string?>())).ReturnsAsync(user);
            _passwordServiceMock.Setup(ps => ps.VerifyPassword(It.IsAny<string>(), user.passwordHash)).Returns(true);
            _tokenCacheRepositoryMock.Setup(tc => tc.AddRefreshTokenAsync(user.userId, It.IsAny<string>())).Returns(Task.CompletedTask);

            var (_, refreshToken, _) = await _authService.LoginAsync(user.email, "Password123");

            _tokenCacheRepositoryMock.Reset();
            _tokenCacheRepositoryMock.Setup(tc => tc.IsRefreshTokenValidAsync(user.userId, refreshToken)).ReturnsAsync(true);

            // Act
            var isValid = await _authService.ValidateRefreshTokenAsync(refreshToken);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public async Task ValidateRefreshTokenAsync_ShouldReturnFalse_WhenTokenNotWhitelisted()
        {
            // Arrange
            var user = CreateTestUser("refreshinvalid@example.com", UserStatus.Active);
            _userRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<string?>())).ReturnsAsync(user);
            _passwordServiceMock.Setup(ps => ps.VerifyPassword(It.IsAny<string>(), user.passwordHash)).Returns(true);
            _tokenCacheRepositoryMock.Setup(tc => tc.AddRefreshTokenAsync(user.userId, It.IsAny<string>())).Returns(Task.CompletedTask);

            var (_, refreshToken, _) = await _authService.LoginAsync(user.email, "Password123");

            _tokenCacheRepositoryMock.Reset();
            _tokenCacheRepositoryMock.Setup(tc => tc.IsRefreshTokenValidAsync(user.userId, refreshToken)).ReturnsAsync(false);

            // Act
            var isValid = await _authService.ValidateRefreshTokenAsync(refreshToken);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public async Task LogoutAsync_ShouldRevokeTokensSuccessfully()
        {
            // Arrange
            var user = CreateTestUser("logout@example.com", UserStatus.Active);
            _userRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<string?>())).ReturnsAsync(user);
            _passwordServiceMock.Setup(ps => ps.VerifyPassword(It.IsAny<string>(), user.passwordHash)).Returns(true);
            _tokenCacheRepositoryMock.Setup(tc => tc.AddRefreshTokenAsync(user.userId, It.IsAny<string>())).Returns(Task.CompletedTask);

            var (accessToken, refreshToken, _) = await _authService.LoginAsync(user.email, "Password123");

            _tokenCacheRepositoryMock.Reset();
            _tokenCacheRepositoryMock.Setup(tc => tc.RevokeAccessTokenAsync(It.IsAny<string>(), It.IsAny<TimeSpan>())).Returns(Task.CompletedTask);
            _tokenCacheRepositoryMock.Setup(tc => tc.RemoveRefreshTokenAsync(user.userId, refreshToken)).Returns(Task.CompletedTask);

            // Act
            await _authService.LogoutAsync(accessToken, refreshToken);

            // Assert
            _tokenCacheRepositoryMock.Verify(tc => tc.RevokeAccessTokenAsync(It.IsAny<string>(), It.IsAny<TimeSpan>()), Times.Once);
            _tokenCacheRepositoryMock.Verify(tc => tc.RemoveRefreshTokenAsync(user.userId, refreshToken), Times.Once);
        }

        [Fact]
        public async Task FirebaseLoginAsync_ShouldReturnTokens_WhenExistingUser()
        {
            // Arrange
            var firebaseToken = "validFirebaseToken";
            var email = "firebaseuser@example.com";
            var user = CreateTestUser(email, UserStatus.Active);

            _firebaseServiceMock.Setup(fs => fs.GetUserInfoFromTokenAsync(firebaseToken))
                .ReturnsAsync((email, user.fullName, "http://avatar"));

            _userRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<string?>()))
                .ReturnsAsync(user);

            _userRepositoryMock.Setup(r => r.UpdateAsync(user)).Returns(Task.CompletedTask);
            _userRepositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
            _tokenCacheRepositoryMock.Setup(tc => tc.AddRefreshTokenAsync(user.userId, It.IsAny<string>())).Returns(Task.CompletedTask);

            // Act
            var (accessToken, refreshToken, dto) = await _authService.FirebaseLoginAsync(firebaseToken);

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(accessToken));
            Assert.False(string.IsNullOrWhiteSpace(refreshToken));
            Assert.Equal(user.userId, dto.Id);
        }

        [Fact]
        public async Task FirebaseLoginAsync_ShouldThrow_WhenFirebaseAuthFails()
        {
            // Arrange
            var firebaseToken = "badToken";
            _firebaseServiceMock.Setup(fs => fs.GetUserInfoFromTokenAsync(firebaseToken))
                .ThrowsAsync(new UnauthorizedAccessException());

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ApiException>(() => _authService.FirebaseLoginAsync(firebaseToken));
            Assert.Equal("FIREBASE_AUTH_FAILED", ex.ErrorCode);
        }

        private static User CreateTestUser(string email, UserStatus status)
        {
            return new User
            {
                userId = Guid.NewGuid(),
                fullName = "Test User",
                email = email,
                passwordHash = "hashedPassword",
                phone = null,
                avatarUrl = null,
                credit = 0,
                createdAt = DateTime.UtcNow,
                lastLogin = DateTime.UtcNow,
                status = status,
                roleId = Guid.NewGuid(),
                Role = new Role { roleId = Guid.NewGuid(), roleName = "STUDENT" }
            };
        }
    }
} 