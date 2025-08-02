namespace JCertPreApplication.Application.Dtos.Auth
{
    /// <summary>
    /// 📝 Data stored in Redis cache for password reset token.
    /// </summary>
    public class PasswordResetTokenData
    {
        /// <summary>
        /// User ID associated with the reset request.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Email address of the user.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Timestamp when the token was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// IP address from where the reset was requested (for security logging).
        /// </summary>
        public string? IpAddress { get; set; }

        /// <summary>
        /// Whether this token has been used (for one-time use enforcement).
        /// </summary>
        public bool IsUsed { get; set; } = false;

        /// <summary>
        /// Timestamp when the token was used (if applicable).
        /// </summary>
        public DateTime? UsedAt { get; set; }
    }
}
