using FluentAssertions;
using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.Auth;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Application.Features.Auth;
using JCertPreApplication.Domain.Configuration;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.UnitTests.Common.Builders;
using JCertPreApplication.UnitTests.Common.Extensions;
using JCertPreApplication.UnitTests.Common.Helpers;
using Microsoft.Extensions.Options;
using Moq;

namespace JCertPreApplication.UnitTests.Features.Auth;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IRoleRepository> _mockRoleRepository;
    private readonly Mock<IFirebaseService> _mockFirebaseService;
    private readonly Mock<IPasswordService> _mockPasswordService;
    private readonly Mock<ITokenCacheRepository> _mockTokenCacheRepository;
    private readonly Mock<ICacheRepository> _mockCacheRepository;
    private readonly Mock<IFileService> _mockFileService;
    private readonly Mock<IMailService> _mockMailService;
    private readonly Mock<IOptions<JwtConfiguration>> _mockJwtOptions;
    private readonly Mock<IOptions<FrontendConfiguration>> _mockFrontendOptions;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockRoleRepository = new Mock<IRoleRepository>();
        _mockFirebaseService = new Mock<IFirebaseService>();
        _mockPasswordService = new Mock<IPasswordService>();
        _mockTokenCacheRepository = new Mock<ITokenCacheRepository>();
        _mockCacheRepository = new Mock<ICacheRepository>();
        _mockFileService = new Mock<IFileService>();
        _mockMailService = new Mock<IMailService>();
        _mockJwtOptions = new Mock<IOptions<JwtConfiguration>>();
        _mockFrontendOptions = new Mock<IOptions<FrontendConfiguration>>();

        // Setup configuration mocks
        _mockJwtOptions.Setup(x => x.Value).Returns(new JwtConfiguration
        {
            SecretKey = "test-secret-key-that-is-long-enough-for-jwt-signing-algorithm",
            RefreshSecretKey = "test-refresh-secret-key-that-is-long-enough-for-jwt-signing-algorithm",
            Issuer = "test-issuer",
            Audience = "test-audience",
            ExpiryInMinutes = 60
        });

        _mockFrontendOptions.Setup(x => x.Value).Returns(new FrontendConfiguration
        {
            BaseUrl = "http://localhost:3000"
        });

        _authService = new AuthService(
            _mockUserRepository.Object,
            _mockRoleRepository.Object,
            _mockFirebaseService.Object,
            _mockPasswordService.Object,
            _mockTokenCacheRepository.Object,
            _mockCacheRepository.Object,
            _mockFileService.Object,
            _mockMailService.Object,
            _mockJwtOptions.Object,
            _mockFrontendOptions.Object);
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ShouldReturnTokensAndUser()
    {
        // Arrange
        var email = "test@example.com";
        var password = "password123";
        var role = new Role { roleId = Guid.NewGuid(), roleName = "STUDENT" };
        var user = UserBuilder.Create()
            .WithEmail(email)
            .WithRole(role)
            .Build();

        _mockUserRepository.SetupUserRepository(user);
        _mockPasswordService.Setup(x => x.VerifyPassword(password, user.passwordHash))
                           .Returns(true);

        // Act
        var result = await _authService.LoginAsync(email, password);

        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
        result.User.Should().NotBeNull();
        result.User.Email.Should().Be(email);
        result.User.FullName.Should().Be(user.fullName);

        // Verify method calls
        _mockUserRepository.Verify(x => x.GetByEmailWithRoleAsync(email), Times.Once);
        _mockPasswordService.Verify(x => x.VerifyPassword(password, user.passwordHash), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidEmail_ShouldThrowUnauthorizedException()
    {
        // Arrange
        var email = "nonexistent@example.com";
        var password = "password123";

        _mockUserRepository.SetupUserRepositoryNotFound(email);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _authService.LoginAsync(email, password));

        exception.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
        exception.ErrorCode.Should().Be("INVALID_CREDENTIALS");
        exception.Message.Should().Be("Invalid email or password.");

        _mockUserRepository.Verify(x => x.GetByEmailWithRoleAsync(email), Times.Once);
        _mockPasswordService.Verify(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ShouldThrowUnauthorizedException()
    {
        // Arrange
        var email = "test@example.com";
        var password = "wrongpassword";
        var role = new Role { roleId = Guid.NewGuid(), roleName = "STUDENT" };
        var user = UserBuilder.Create()
            .WithEmail(email)
            .WithRole(role)
            .Build();

        _mockUserRepository.SetupUserRepository(user);
        _mockPasswordService.Setup(x => x.VerifyPassword(password, user.passwordHash))
                           .Returns(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _authService.LoginAsync(email, password));

        exception.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
        exception.ErrorCode.Should().Be("INVALID_CREDENTIALS");

        _mockUserRepository.Verify(x => x.GetByEmailWithRoleAsync(email), Times.Once);
        _mockPasswordService.Verify(x => x.VerifyPassword(password, user.passwordHash), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WithInactiveUser_ShouldThrowUnauthorizedException()
    {
        // Arrange
        var email = "test@example.com";
        var password = "password123";
        var role = new Role { roleId = Guid.NewGuid(), roleName = "STUDENT" };
        var user = UserBuilder.Create()
            .WithEmail(email)
            .WithRole(role)
            .AsInactive()
            .Build();

        _mockUserRepository.SetupUserRepository(user);
        _mockPasswordService.Setup(x => x.VerifyPassword(password, user.passwordHash))
                           .Returns(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _authService.LoginAsync(email, password));

        exception.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);
        exception.ErrorCode.Should().Be("ACCOUNT_INACTIVE");
    }

    #region Registration Tests

    [Fact]
    public async Task RegisterAsync_WithValidData_ShouldReturnTokens()
    {
        // Arrange
        var registerModel = new RegisterModel
        {
            FullName = "Test User",
            Email = "newuser@example.com",
            Password = "Password123!",
            Phone = "0123456789"
        };

        var defaultRole = new Role { roleId = Guid.NewGuid(), roleName = "STUDENT" };

        _mockUserRepository.SetupUserRepositoryNotFound(registerModel.Email);
        _mockRoleRepository.Setup(x => x.GetByRoleNameAsync("STUDENT"))
                          .ReturnsAsync(defaultRole);
        _mockPasswordService.Setup(x => x.HashPassword(registerModel.Password))
                           .Returns("hashed-password");
        _mockUserRepository.Setup(x => x.InsertAsync(It.IsAny<User>()))
                          .ReturnsAsync(It.IsAny<User>());
        _mockUserRepository.Setup(x => x.SaveChangesAsync())
                          .ReturnsAsync(1);

        // Act
        var result = await _authService.RegisterAsync(registerModel);

        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
        result.User.Should().NotBeNull();
        result.User.Email.Should().Be(registerModel.Email);
        result.User.FullName.Should().Be(registerModel.FullName);

        _mockUserRepository.Verify(x => x.GetByEmailWithRoleAsync(registerModel.Email), Times.Once);
        _mockPasswordService.Verify(x => x.HashPassword(registerModel.Password), Times.Once);
        _mockUserRepository.Verify(x => x.InsertAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_WithExistingEmail_ShouldThrowBadRequestException()
    {
        // Arrange
        var registerModel = new RegisterModel
        {
            FullName = "Test User",
            Email = "existing@example.com",
            Password = "Password123!"
        };

        var existingUser = UserBuilder.Create()
            .WithEmail(registerModel.Email)
            .Build();

        _mockUserRepository.SetupUserRepository(existingUser);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _authService.RegisterAsync(registerModel));

        exception.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        exception.ErrorCode.Should().Be("EMAIL_ALREADY_EXISTS");

        _mockUserRepository.Verify(x => x.GetByEmailWithRoleAsync(registerModel.Email), Times.Once);
        _mockPasswordService.Verify(x => x.HashPassword(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_WithInvalidData_ShouldThrowValidationException()
    {
        // Arrange - Missing role scenario
        var registerModel = new RegisterModel
        {
            FullName = "Test User",
            Email = "test@example.com",
            Password = "Password123!"
        };

        _mockUserRepository.SetupUserRepositoryNotFound(registerModel.Email);
        _mockRoleRepository.Setup(x => x.GetByRoleNameAsync("STUDENT"))
                          .ReturnsAsync((Role?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _authService.RegisterAsync(registerModel));

        exception.StatusCode.Should().Be(System.Net.HttpStatusCode.InternalServerError);
        exception.ErrorCode.Should().Be("DEFAULT_ROLE_NOT_FOUND");

        _mockRoleRepository.Verify(x => x.GetByRoleNameAsync("STUDENT"), Times.Once);
    }

    #endregion

    #region Refresh Token Tests

    [Fact]
    public async Task RefreshTokenAsync_WithValidToken_ShouldReturnNewTokens()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var accessJti = Guid.NewGuid().ToString();
        var accessToken = JwtTestHelper.CreateAccessToken(userId, "test@example.com", "Student", accessJti);
        var refreshToken = JwtTestHelper.CreateRefreshToken(userId);

        var role = new Role { roleId = Guid.NewGuid(), roleName = "STUDENT" };
        var user = UserBuilder.Create()
            .WithId(userId)
            .WithEmail("test@example.com")
            .WithRole(role)
            .Build();

        _mockTokenCacheRepository.Setup(x => x.IsRefreshTokenValidAsync(userId, refreshToken))
                                 .ReturnsAsync(true);
        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
                          .ReturnsAsync(user);
        _mockTokenCacheRepository.Setup(x => x.RevokeAccessTokenAsync(accessJti, It.IsAny<TimeSpan>()))
                                 .Returns(Task.CompletedTask);
        _mockTokenCacheRepository.Setup(x => x.RemoveRefreshTokenAsync(userId, refreshToken))
                                 .Returns(Task.CompletedTask);
        _mockTokenCacheRepository.Setup(x => x.AddRefreshTokenAsync(userId, It.IsAny<string>()))
                                 .Returns(Task.CompletedTask);

        // Act
        var result = await _authService.RefreshTokenAsync(accessToken, refreshToken);

        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
        result.User.Should().NotBeNull();

        _mockTokenCacheRepository.Verify(x => x.RevokeAccessTokenAsync(accessJti, It.IsAny<TimeSpan>()), Times.Once);
        _mockTokenCacheRepository.Verify(x => x.RemoveRefreshTokenAsync(userId, refreshToken), Times.Once);
    }

    [Fact]
    public async Task RefreshTokenAsync_WithExpiredToken_ShouldThrowUnauthorizedException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var accessJti = Guid.NewGuid().ToString();
        var accessToken = JwtTestHelper.CreateAccessToken(userId, "test@example.com", "Student", accessJti);
        var refreshToken = JwtTestHelper.CreateExpiredRefreshToken(userId);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _authService.RefreshTokenAsync(accessToken, refreshToken));

        exception.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
        exception.ErrorCode.Should().Be("INVALID_ACCESS_TOKEN");
    }

    #endregion

    #region Logout Tests

    [Fact]
    public async Task LogoutAsync_WithValidToken_ShouldRevokeToken()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var jti = Guid.NewGuid().ToString();
        var accessToken = JwtTestHelper.CreateAccessToken(userId, "test@example.com", "Student", jti);
        var refreshToken = JwtTestHelper.CreateRefreshToken(userId);

        _mockTokenCacheRepository.Setup(x => x.RevokeAccessTokenAsync(jti, It.IsAny<TimeSpan>()))
                                 .Returns(Task.CompletedTask);
        _mockTokenCacheRepository.Setup(x => x.RemoveRefreshTokenAsync(userId, refreshToken))
                                 .Returns(Task.CompletedTask);

        // Act
        await _authService.LogoutAsync(accessToken, refreshToken);

        // Assert
        _mockTokenCacheRepository.Verify(x => x.RevokeAccessTokenAsync(jti, It.IsAny<TimeSpan>()), Times.Once);
        _mockTokenCacheRepository.Verify(x => x.RemoveRefreshTokenAsync(userId, refreshToken), Times.Once);
    }

    #endregion

    #region Password Reset Tests

    [Fact]
    public async Task ForgotPasswordAsync_WithValidEmail_ShouldSendResetEmail()
    {
        // Arrange
        var email = "user@example.com";
        var ipAddress = "192.168.1.1";
        var user = UserBuilder.Create().WithEmail(email).Build();

        _mockUserRepository.Setup(x => x.GetByEmailAsync(email))
                          .ReturnsAsync(user);
        _mockCacheRepository.Setup(x => x.SetAsync(It.IsAny<string>(), It.IsAny<PasswordResetTokenData>(), It.IsAny<TimeSpan>()))
                           .Returns(Task.CompletedTask);
        _mockMailService.Setup(x => x.SendTemplateEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>()))
                       .Returns(Task.CompletedTask);

        // Act
        var result = await _authService.ForgotPasswordAsync(email, ipAddress);

        // Assert
        result.Should().NotBeNullOrEmpty();

        _mockUserRepository.Verify(x => x.GetByEmailAsync(email), Times.Once);
        _mockCacheRepository.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<PasswordResetTokenData>(), It.IsAny<TimeSpan>()), Times.Once);
        _mockMailService.Verify(x => x.SendTemplateEmailAsync(user.email, "password-reset", It.IsAny<object>()), Times.Once);
    }

    [Fact]
    public async Task ResetPasswordAsync_WithValidToken_ShouldUpdatePassword()
    {
        // Arrange
        var resetToken = "valid-reset-token";
        var newPassword = "NewSecurePassword123!";
        var userId = Guid.NewGuid();
        var user = UserBuilder.Create().WithEmail("test@example.com").Build();
        user.userId = userId;
        
        var resetData = CacheTestHelper.CreatePasswordResetTokenData(userId, user.email);

        _mockCacheRepository.SetupPasswordResetTokenData(resetToken, resetData);
        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
                          .ReturnsAsync(user);
        _mockPasswordService.Setup(x => x.HashPassword(newPassword))
                           .Returns("new_hashed_password");
        _mockUserRepository.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                          .Returns(Task.CompletedTask);
        _mockUserRepository.Setup(x => x.SaveChangesAsync())
                          .ReturnsAsync(1);
        _mockCacheRepository.Setup(x => x.SetAsync(It.IsAny<string>(), It.IsAny<PasswordResetTokenData>(), It.IsAny<TimeSpan>()))
                           .Returns(Task.CompletedTask);

        // Act
        var result = await _authService.ResetPasswordAsync(resetToken, newPassword);

        // Assert
        result.Should().NotBeNullOrEmpty();

        _mockPasswordService.Verify(x => x.HashPassword(newPassword), Times.Once);
        _mockUserRepository.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Once);
        _mockUserRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    #endregion
}
