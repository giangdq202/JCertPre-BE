namespace JCertPreApplication.Application.Dtos.File
{
    /// <summary>
    /// Generic file deletion result that doesn't depend on any specific file storage provider
    /// </summary>
    public class FileDeletionResult
    {
        public bool Success { get; set; }
        public string PublicId { get; set; } = string.Empty;
        public string? Result { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
