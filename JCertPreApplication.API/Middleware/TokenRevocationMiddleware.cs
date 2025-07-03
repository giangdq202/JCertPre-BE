using JCertPreApplication.Application.Contracts;
using System.IdentityModel.Tokens.Jwt;

namespace JCertPreApplication.API.Middleware
{
    public class TokenRevocationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TokenRevocationMiddleware> _logger;

        public TokenRevocationMiddleware(RequestDelegate next, ILogger<TokenRevocationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, ITokenCacheRepository tokenCacheRepository)
        {
            // Skip check for non-authenticated endpoints
            var authorizationHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            
            if (!string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer "))
            {
                var token = authorizationHeader.Substring("Bearer ".Length).Trim();
                
                try
                {
                    // Parse token to get JTI without validating signature (that's done by JWT middleware)
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var jsonToken = tokenHandler.ReadJwtToken(token);
                    
                    var jti = jsonToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
                    
                    if (!string.IsNullOrEmpty(jti))
                    {
                        // Check if token is revoked
                        var isRevoked = await tokenCacheRepository.IsAccessTokenRevokedAsync(jti);
                        
                        if (isRevoked)
                        {
                            _logger.LogWarning("Revoked access token attempted to access {Path}. JTI: {Jti}", 
                                context.Request.Path, jti);
                            
                            context.Response.StatusCode = 401;
                            await context.Response.WriteAsync("Access token has been revoked");
                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    // If we can't parse the token, let JWT middleware handle it
                    _logger.LogDebug("Could not parse token for revocation check: {Error}", ex.Message);
                }
            }

            await _next(context);
        }
    }
} 