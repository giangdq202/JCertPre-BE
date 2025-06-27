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
        public string passwordHash { get; set; }

        [Required(ErrorMessage = "Phone is required")]
        public string phone { get; set; }
    }
}
