namespace JCertPreApplication.Application.Dtos.Auth
{
    /// <summary>
    /// 🔐 User information for authentication response.
    /// </summary>
    /// <remarks>
    /// This DTO contains only essential user information returned after successful authentication.
    /// It's separate from AppUserDto to avoid exposing unnecessary fields in auth responses.
    /// </remarks>
    public class AuthUserDto
    {
        /// <summary>
        /// Unique user identifier.
        /// </summary>
        /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
        public Guid Id { get; set; }

        /// <summary>
        /// User's full name.
        /// </summary>
        /// <example>Tanaka Hiroshi</example>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// User's email address.
        /// </summary>
        /// <example>tanaka.hiroshi@jcertpre.com</example>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// User's phone number (optional).
        /// </summary>
        /// <example>+84-90-123-4567</example>
        public string? Phone { get; set; }
        
        /// <summary>
        /// User's avatar URL (optional).
        /// </summary>
        public string? AvatarUrl { get; set; }
        
        /// <summary>
        /// User's credit amount.
        /// </summary>
        public int Credit { get; set; }
        
        /// <summary>
        /// User's role name.
        /// </summary>
        public string RoleName { get; set; } = string.Empty;
    }
} 