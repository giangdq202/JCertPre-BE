namespace JCertPreApplication.Application.Dtos.Auth
{
    /// <summary>
    /// DTO representing a verified Firebase token without direct dependency on Firebase SDK
    /// This allows the Application layer to remain infrastructure-agnostic
    /// </summary>
    public class FirebaseTokenDto
    {
        public string Uid { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Picture { get; set; }
        public Dictionary<string, object> Claims { get; set; } = new();
        public DateTime IssuedAt { get; set; }
        public DateTime ExpirationTime { get; set; }
    }
} 