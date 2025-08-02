using System.ComponentModel.DataAnnotations;

namespace JCertPreApplication.Application.Dtos.Auth
{
    /// <summary>
    /// 📧 Request to initiate password reset process.
    /// </summary>
    public class ForgotPasswordRequest
    {
        /// <summary>
        /// User's email address to send reset instructions.
        /// </summary>
        /// <example>user@jcertpre.com</example>
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please provide a valid email address")]
        [StringLength(254, ErrorMessage = "Email cannot exceed 254 characters")]
        public string Email { get; set; } = string.Empty;
    }
}
