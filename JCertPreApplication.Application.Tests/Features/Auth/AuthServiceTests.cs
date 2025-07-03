using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
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