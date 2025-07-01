using System.ComponentModel.DataAnnotations;

namespace JCertPreApplication.Application.Dtos.Auth
{
    /// <summary>
    /// 🔐 Login credentials data transfer object.
    /// </summary>
    /// <remarks>
    /// This DTO contains the required information for user authentication.
    /// Used for standard email/password login functionality.
    /// </remarks>
    public class LoginModel
    {
        /// <summary>
        /// User's email address (used as username).
        /// </summary>
        /// <example>user@jcertpre.com</example>
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please provide a valid email address")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// User's password.
        /// </summary>
        /// <example>MySecurePassword123!</example>
        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters")]
        public string Password { get; set; } = string.Empty;
    }
}
