using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace JCertPreApplication.Application.Dtos.Auth
{
    /// <summary>
    /// 📝 User registration data transfer object.
    /// </summary>
    /// <remarks>
    /// This DTO contains all required information for creating a new user account.
    /// Includes validation attributes to ensure data integrity and security.
    /// </remarks>
    public class RegisterModel
    {
        /// <summary>
        /// User's email address (will be used as login username).
        /// </summary>
        /// <example>newuser@jcertpre.com</example>
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please provide a valid email address")]
        [StringLength(254, ErrorMessage = "Email cannot exceed 254 characters")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// User's password for account security.
        /// </summary>
        /// <example>MyStrongPassword123!</example>
        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// User's full name for profile and display purposes.
        /// </summary>
        /// <example>Yamada Taro</example>
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Full name must be between 2 and 100 characters")]
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// User's phone number (optional). Include country code if international.
        /// </summary>
        /// <example>+81-90-1234-5678</example>
        [Phone(ErrorMessage = "Please provide a valid phone number")]
        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        public string? Phone { get; set; }

        /// <summary>
        /// Avatar image file (optional). Supports common image formats.
        /// </summary>
        public IFormFile? AvatarFile { get; set; }
    }
}
