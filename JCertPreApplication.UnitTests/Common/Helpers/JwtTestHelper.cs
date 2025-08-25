using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JCertPreApplication.UnitTests.Common.Helpers;

public static class JwtTestHelper
{
    private const string TestSecretKey = "test-secret-key-that-is-long-enough-for-jwt-signing-algorithm";
    private const string TestRefreshSecretKey = "test-refresh-secret-key-that-is-long-enough-for-jwt-signing-algorithm";
    private const string TestIssuer = "test-issuer";
    private const string TestAudience = "test-audience";

    /// <summary>
    /// Creates a valid access token for testing purposes
    /// </summary>
    public static string CreateAccessToken(
        Guid userId,
        string email = "test@example.com",
        string role = "Student",
        string jti = null,
        DateTime? expiryTime = null)
    {
        jti ??= Guid.NewGuid().ToString();
        expiryTime ??= DateTime.UtcNow.AddHours(1);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, jti),
            new Claim(JwtRegisteredClaimNames.Iat, 
                new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(), 
                ClaimValueTypes.Integer64),
            new Claim("type", "access")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TestSecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: TestIssuer,
            audience: TestAudience,
            claims: claims,
            expires: expiryTime,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Creates a valid refresh token for testing purposes
    /// </summary>
    public static string CreateRefreshToken(
        Guid userId,
        string email = "test@example.com",
        DateTime? expiryTime = null)
    {
        expiryTime ??= DateTime.UtcNow.AddDays(7);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, 
                new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(), 
                ClaimValueTypes.Integer64),
            new Claim("type", "refresh")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TestRefreshSecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: TestIssuer,
            audience: TestAudience,
            claims: claims,
            expires: expiryTime,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Creates an expired access token for testing purposes
    /// </summary>
    public static string CreateExpiredAccessToken(Guid userId, string email = "test@example.com")
    {
        return CreateAccessToken(userId, email, expiryTime: DateTime.UtcNow.AddMinutes(-30));
    }

    /// <summary>
    /// Creates an expired refresh token for testing purposes
    /// </summary>
    public static string CreateExpiredRefreshToken(Guid userId, string email = "test@example.com")
    {
        return CreateRefreshToken(userId, email, expiryTime: DateTime.UtcNow.AddMinutes(-30));
    }

    /// <summary>
    /// Extracts JTI from a JWT token
    /// </summary>
    public static string ExtractJti(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadJwtToken(token);
        return jsonToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
    }

    /// <summary>
    /// Extracts UserId from a JWT token
    /// </summary>
    public static Guid ExtractUserId(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadJwtToken(token);
        var userIdClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim);
    }
}
