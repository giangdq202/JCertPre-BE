using JCertPreApplication.Application.Dtos.Auth;
using Moq;
using JCertPreApplication.Application.Contracts;

namespace JCertPreApplication.UnitTests.Common.Helpers;

public static class CacheTestHelper
{
    /// <summary>
    /// Sets up cache repository mock to return password reset token data
    /// </summary>
    public static void SetupPasswordResetTokenData(
        this Mock<ICacheRepository> mockCacheRepository,
        string token,
        PasswordResetTokenData tokenData)
    {
        var cacheKey = $"password-reset:{token}";
        mockCacheRepository
            .Setup(x => x.GetAsync<PasswordResetTokenData>(cacheKey))
            .ReturnsAsync(tokenData);
    }

    /// <summary>
    /// Sets up cache repository mock to return null for invalid token
    /// </summary>
    public static void SetupPasswordResetTokenNotFound(
        this Mock<ICacheRepository> mockCacheRepository,
        string token)
    {
        var cacheKey = $"password-reset:{token}";
        mockCacheRepository
            .Setup(x => x.GetAsync<PasswordResetTokenData>(cacheKey))
            .ReturnsAsync((PasswordResetTokenData?)null);
    }

    /// <summary>
    /// Creates a valid password reset token data for testing
    /// </summary>
    public static PasswordResetTokenData CreatePasswordResetTokenData(
        Guid userId,
        string email,
        string ipAddress = "127.0.0.1",
        bool isUsed = false,
        DateTime? createdAt = null)
    {
        return new PasswordResetTokenData
        {
            UserId = userId,
            Email = email,
            CreatedAt = createdAt ?? DateTime.UtcNow.AddMinutes(-5),
            IpAddress = ipAddress,
            IsUsed = isUsed,
            UsedAt = isUsed ? DateTime.UtcNow : null
        };
    }
}
