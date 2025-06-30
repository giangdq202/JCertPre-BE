using System.ComponentModel.DataAnnotations;

namespace JCertPreApplication.Application.Dtos.Auth
{
    public class RefreshTokenModel
    {
        [Required(ErrorMessage = "Access token is required")]
        public string AccessToken { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Refresh token is required")]
        public string RefreshToken { get; set; } = string.Empty;
    }
} 