namespace JCertPreApplication.Application.Dtos.Auth
{
    public class LogoutModel
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
} 