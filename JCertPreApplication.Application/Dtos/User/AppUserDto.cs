namespace JCertPreApplication.Application.Dtos.User
{
    /// <summary>
    /// 👤 User information data transfer object.
    /// </summary>
    /// <remarks>
    /// This DTO contains essential user information used across different contexts
    /// such as course instructors, user profiles, and general user references.
    /// </remarks>
    public class AppUserDto
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
        public string fullName { get; set; } = string.Empty;

        /// <summary>
        /// User's email address.
        /// </summary>
        /// <example>tanaka.hiroshi@jcertpre.com</example>
        public string email { get; set; } = string.Empty;

        /// <summary>
        /// User's phone number (optional).
        /// </summary>
        /// <example>+84-90-123-4567</example>
        public string? phone { get; set; }
    }
}
