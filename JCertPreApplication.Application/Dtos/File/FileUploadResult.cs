namespace JCertPreApplication.Application.Dtos.File
{
    /// <summary>
    /// Generic file upload result that doesn't depend on any specific file storage provider
    /// </summary>
    public class FileUploadResult
    {
        public bool Success { get; set; } = true;
        public string PublicId { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string? SecureUrl { get; set; }
        public long Bytes { get; set; }
        public string? Format { get; set; }
        public string? ResourceType { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? ErrorMessage { get; set; }
        
        // Additional metadata
        public Dictionary<string, object> Metadata { get; set; } = new();
    }
}
