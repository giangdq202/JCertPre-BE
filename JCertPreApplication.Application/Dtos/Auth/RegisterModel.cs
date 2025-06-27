using System.ComponentModel.DataAnnotations;

namespace JCertPreApplication.Application.Dtos.Auth
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Full name is required")]
        public string fullName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
        public string password { get; set; }

        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        public string? phone { get; set; }

        [StringLength(500, ErrorMessage = "Avatar URL cannot exceed 500 characters")]
        public string? avatarUrl { get; set; }
    }
}
