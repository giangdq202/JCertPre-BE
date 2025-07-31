namespace JCertPreApplication.Domain.Configuration
{
    /// <summary>
    /// Configuration class for Appwrite service integration
    /// </summary>
    public class AppwriteConfiguration
    {
        public const string SectionName = "Appwrite";

        /// <summary>
        /// Appwrite server endpoint URL
        /// </summary>
        public string Endpoint { get; set; } = string.Empty;

        /// <summary>
        /// Appwrite project ID
        /// </summary>
        public string ProjectId { get; set; } = string.Empty;

        /// <summary>
        /// Appwrite API key with Storage permissions
        /// </summary>
        public string ApiKey { get; set; } = string.Empty;

        /// <summary>
        /// Bucket ID for image files
        /// </summary>
        public string ImagesBucketId { get; set; } = "images";

        /// <summary>
        /// Bucket ID for video files
        /// </summary>
        public string VideosBucketId { get; set; } = "videos";

        /// <summary>
        /// Bucket ID for document files
        /// </summary>
        public string DocumentsBucketId { get; set; } = "documents";

        /// <summary>
        /// Use HTTPS for file URLs
        /// </summary>
        public bool Secure { get; set; } = true;

        /// <summary>
        /// Maximum file size in MB
        /// </summary>
        public int MaxFileSizeMB { get; set; } = 10;

        /// <summary>
        /// Upload timeout in seconds
        /// </summary>
        public int UploadTimeoutSeconds { get; set; } = 600;

        /// <summary>
        /// Validates the configuration settings
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when required settings are missing</exception>
        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(Endpoint))
                throw new InvalidOperationException("Appwrite Endpoint is required. Please configure Appwrite__Endpoint in your .env file.");

            if (string.IsNullOrWhiteSpace(ProjectId))
                throw new InvalidOperationException("Appwrite ProjectId is required. Please configure Appwrite__ProjectId in your .env file.");

            if (string.IsNullOrWhiteSpace(ApiKey))
                throw new InvalidOperationException("Appwrite ApiKey is required. Please configure Appwrite__ApiKey in your .env file.");

            if (string.IsNullOrWhiteSpace(ImagesBucketId))
                throw new InvalidOperationException("Appwrite ImagesBucketId is required. Please configure Appwrite__ImagesBucketId in your .env file.");

            if (string.IsNullOrWhiteSpace(VideosBucketId))
                throw new InvalidOperationException("Appwrite VideosBucketId is required. Please configure Appwrite__VideosBucketId in your .env file.");

            if (string.IsNullOrWhiteSpace(DocumentsBucketId))
                throw new InvalidOperationException("Appwrite DocumentsBucketId is required. Please configure Appwrite__DocumentsBucketId in your .env file.");

            if (MaxFileSizeMB <= 0)
                throw new InvalidOperationException("Appwrite MaxFileSizeMB must be greater than 0.");

            if (UploadTimeoutSeconds <= 0)
                throw new InvalidOperationException("Appwrite UploadTimeoutSeconds must be greater than 0.");
        }
    }
}
