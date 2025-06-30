using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Persistence.Cache;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Net;

namespace JCertPreApplication.Persistence.Repositories
{
    public class TokenCacheRepository : ITokenCacheRepository
    {
        private readonly IDatabase _database;
        private readonly ILogger<TokenCacheRepository> _logger;

        // Redis key patterns
        private const string REFRESH_TOKEN_KEY_PATTERN = "user-refresh-tokens:{0}";
        private const string REVOKED_ACCESS_TOKEN_KEY_PATTERN = "revoked-access-token:{0}";

        public TokenCacheRepository(RedisClient redisClient, ILogger<TokenCacheRepository> logger)
        {
            _database = redisClient.Connection.GetDatabase();
            _logger = logger;
        }

        #region Refresh Token Whitelist Operations

        public async Task AddRefreshTokenAsync(Guid userId, string refreshToken)
        {
            try
            {
                if (userId == Guid.Empty)
                    throw new ApiException(HttpStatusCode.BadRequest, "INVALID_USER_ID", "User ID cannot be empty.");

                if (string.IsNullOrEmpty(refreshToken))
                    throw new ApiException(HttpStatusCode.BadRequest, "INVALID_REFRESH_TOKEN", "Refresh token cannot be null or empty.");

                var key = string.Format(REFRESH_TOKEN_KEY_PATTERN, userId);
                await _database.SetAddAsync(key, refreshToken);

                _logger.LogDebug("Added refresh token to whitelist for user {UserId}", userId);
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add refresh token to whitelist for user {UserId}", userId);
                throw new ApiException(HttpStatusCode.InternalServerError, "CACHE_OPERATION_ERROR", 
                    "Failed to add refresh token to whitelist.");
            }
        }

        public async Task RemoveRefreshTokenAsync(Guid userId, string refreshToken)
        {
            try
            {
                if (userId == Guid.Empty)
                    throw new ApiException(HttpStatusCode.BadRequest, "INVALID_USER_ID", "User ID cannot be empty.");

                if (string.IsNullOrEmpty(refreshToken))
                    throw new ApiException(HttpStatusCode.BadRequest, "INVALID_REFRESH_TOKEN", "Refresh token cannot be null or empty.");

                var key = string.Format(REFRESH_TOKEN_KEY_PATTERN, userId);
                await _database.SetRemoveAsync(key, refreshToken);

                _logger.LogDebug("Removed refresh token from whitelist for user {UserId}", userId);
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to remove refresh token from whitelist for user {UserId}", userId);
                throw new ApiException(HttpStatusCode.InternalServerError, "CACHE_OPERATION_ERROR", 
                    "Failed to remove refresh token from whitelist.");
            }
        }

        public async Task<bool> IsRefreshTokenValidAsync(Guid userId, string refreshToken)
        {
            try
            {
                if (userId == Guid.Empty)
                    throw new ApiException(HttpStatusCode.BadRequest, "INVALID_USER_ID", "User ID cannot be empty.");

                if (string.IsNullOrEmpty(refreshToken))
                    throw new ApiException(HttpStatusCode.BadRequest, "INVALID_REFRESH_TOKEN", "Refresh token cannot be null or empty.");

                var key = string.Format(REFRESH_TOKEN_KEY_PATTERN, userId);
                var isValid = await _database.SetContainsAsync(key, refreshToken);

                _logger.LogDebug("Checked refresh token validity for user {UserId}: {IsValid}", userId, isValid);
                return isValid;
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to check refresh token validity for user {UserId}", userId);
                throw new ApiException(HttpStatusCode.InternalServerError, "CACHE_OPERATION_ERROR", 
                    "Failed to check refresh token validity.");
            }
        }

        public async Task RemoveAllRefreshTokensAsync(Guid userId)
        {
            try
            {
                if (userId == Guid.Empty)
                    throw new ApiException(HttpStatusCode.BadRequest, "INVALID_USER_ID", "User ID cannot be empty.");

                var key = string.Format(REFRESH_TOKEN_KEY_PATTERN, userId);
                await _database.KeyDeleteAsync(key);

                _logger.LogDebug("Removed all refresh tokens for user {UserId}", userId);
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to remove all refresh tokens for user {UserId}", userId);
                throw new ApiException(HttpStatusCode.InternalServerError, "CACHE_OPERATION_ERROR", 
                    "Failed to remove all refresh tokens.");
            }
        }

        #endregion

        #region Access Token Blacklist Operations

        public async Task RevokeAccessTokenAsync(string jti, TimeSpan remainingLifetime)
        {
            try
            {
                if (string.IsNullOrEmpty(jti))
                    throw new ApiException(HttpStatusCode.BadRequest, "INVALID_JTI", "JWT ID cannot be null or empty.");

                if (remainingLifetime <= TimeSpan.Zero)
                {
                    _logger.LogDebug("Access token {Jti} already expired, skipping revocation", jti);
                    return;
                }

                var key = string.Format(REVOKED_ACCESS_TOKEN_KEY_PATTERN, jti);
                await _database.StringSetAsync(key, "revoked", remainingLifetime);

                _logger.LogDebug("Revoked access token {Jti} with TTL {RemainingLifetime}", jti, remainingLifetime);
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to revoke access token {Jti}", jti);
                throw new ApiException(HttpStatusCode.InternalServerError, "CACHE_OPERATION_ERROR", 
                    "Failed to revoke access token.");
            }
        }

        public async Task<bool> IsAccessTokenRevokedAsync(string jti)
        {
            try
            {
                if (string.IsNullOrEmpty(jti))
                    throw new ApiException(HttpStatusCode.BadRequest, "INVALID_JTI", "JWT ID cannot be null or empty.");

                var key = string.Format(REVOKED_ACCESS_TOKEN_KEY_PATTERN, jti);
                var isRevoked = await _database.KeyExistsAsync(key);

                _logger.LogDebug("Checked access token revocation status for {Jti}: {IsRevoked}", jti, isRevoked);
                return isRevoked;
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to check access token revocation status for {Jti}", jti);
                throw new ApiException(HttpStatusCode.InternalServerError, "CACHE_OPERATION_ERROR", 
                    "Failed to check access token revocation status.");
            }
        }

        #endregion
    }
} 