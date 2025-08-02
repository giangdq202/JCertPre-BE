using System.ComponentModel.DataAnnotations;

namespace JCertPreApplication.Application.Dtos.Auth
{
    /// <summary>
    /// 🔒 Request to reset user password with token.
    /// </summary>
    public class ResetPasswordRequest
    {
        /// <summary>
        /// Reset token received via email.
        /// </summary>
        /// <example>a1b2c3d4e5f6...</example>
        [Required(ErrorMessage = "Reset token is required")]
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// New password for the user account.
        /// </summary>
        /// <example>NewSecurePassword123!</example>
        [Required(ErrorMessage = "New password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters")]
        public string NewPassword { get; set; } = string.Empty;

        /// <summary>
        /// Confirmation of the new password.
        /// </summary>
        /// <example>NewSecurePassword123!</example>
        [Required(ErrorMessage = "Password confirmation is required")]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
