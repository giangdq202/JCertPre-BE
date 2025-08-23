using FluentAssertions;
using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Application.Features.Auth;
using JCertPreApplication.Domain.Configuration;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.UnitTests.Common.Builders;
using JCertPreApplication.UnitTests.Common.Extensions;
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

        exception.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
        exception.ErrorCode.Should().Be("USER_INACTIVE");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task LoginAsync_WithInvalidEmailFormat_ShouldThrowBadRequestException(string email)
    {
        // Arrange
        var password = "password123";

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _authService.LoginAsync(email, password));

        exception.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        exception.ErrorCode.Should().Be("INVALID_INPUT");

        _mockUserRepository.Verify(x => x.GetByEmailWithRoleAsync(It.IsAny<string>()), Times.Never);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task LoginAsync_WithInvalidPassword_ShouldThrowBadRequestException(string password)
    {
        // Arrange
        var email = "test@example.com";

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _authService.LoginAsync(email, password));

        exception.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        exception.ErrorCode.Should().Be("INVALID_INPUT");

        _mockUserRepository.Verify(x => x.GetByEmailWithRoleAsync(It.IsAny<string>()), Times.Never);
    }
}
