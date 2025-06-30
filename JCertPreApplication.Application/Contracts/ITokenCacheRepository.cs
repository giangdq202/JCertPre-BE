namespace JCertPreApplication.Application.Contracts
{
    public interface ITokenCacheRepository
    {
        // Refresh Token Whitelist Operations
        Task AddRefreshTokenAsync(Guid userId, string refreshToken);
        Task RemoveRefreshTokenAsync(Guid userId, string refreshToken);
        Task<bool> IsRefreshTokenValidAsync(Guid userId, string refreshToken);
        Task RemoveAllRefreshTokensAsync(Guid userId);

        // Access Token Blacklist Operations
        Task RevokeAccessTokenAsync(string jti, TimeSpan remainingLifetime);
        Task<bool> IsAccessTokenRevokedAsync(string jti);
    }
} 