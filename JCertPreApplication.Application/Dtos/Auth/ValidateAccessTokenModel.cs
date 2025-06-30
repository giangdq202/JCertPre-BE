using System.ComponentModel.DataAnnotations;

namespace JCertPreApplication.Application.Dtos.Auth
{
    public class ValidateAccessTokenModel
    {
        [Required(ErrorMessage = "Access token is required")]
        public string AccessToken { get; set; } = string.Empty;
    }
} 