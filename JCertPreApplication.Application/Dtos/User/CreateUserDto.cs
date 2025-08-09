using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.Application.Dtos.User
{
    /// <summary>
    /// 👥 Data transfer object for creating a new user account with specified role.
    /// </summary>
    /// <remarks>
    /// This DTO is used by administrators or authorized personnel to create user accounts
    /// directly without going through the registration process. It allows specifying the user's role.
    /// </remarks>
    public class CreateUserDto
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
        /// <example>TempPassword123!</example>
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
        /// User's role in the system. Determines access permissions and features.
        /// </summary>
        /// <example>STUDENT</example>
        [Required(ErrorMessage = "Role name is required")]
        [StringLength(50, ErrorMessage = "Role name cannot exceed 50 characters")]
        public string RoleName { get; set; } = string.Empty;

        /// <summary>
        /// User account status upon creation.
        /// </summary>
        /// <example>Active</example>
        public UserStatus Status { get; set; } = UserStatus.Active;

        /// <summary>
        /// Initial credit amount for the user (default is 0).
        /// </summary>
        /// <example>100</example>
        [Range(0, int.MaxValue, ErrorMessage = "Credit must be a non-negative number")]
        public int Credit { get; set; } = 0;

        /// <summary>
        /// Avatar image file (optional). Supports common image formats.
        /// </summary>
        public IFormFile? AvatarFile { get; set; }
    }
}
