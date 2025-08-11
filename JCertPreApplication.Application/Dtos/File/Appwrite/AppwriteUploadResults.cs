using System.Text.Json.Serialization;

namespace JCertPreApplication.Application.Dtos.File.Appwrite
{
    /// <summary>
    /// Appwrite file upload response for images
    /// </summary>
    public class AppwriteImageUploadResult
    {
        /// <summary>
        /// Appwrite file ID
        /// </summary>
        [JsonPropertyName("fileId")]
        public string FileId { get; set; } = string.Empty;

        /// <summary>
        /// Public ID for compatibility
        /// </summary>
        [JsonPropertyName("publicId")]
        public string PublicId { get; set; } = string.Empty;

        /// <summary>
        /// Original file name
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Bucket ID where file is stored
        /// </summary>
        [JsonPropertyName("bucketId")]
        public string BucketId { get; set; } = string.Empty;

        /// <summary>
        /// File MIME type
        /// </summary>
        [JsonPropertyName("mimeType")]
        public string MimeType { get; set; } = string.Empty;

        /// <summary>
        /// File size in bytes
        /// </summary>
        [JsonPropertyName("sizeOriginal")]
        public long SizeOriginal { get; set; }

        /// <summary>
        /// File extension
        /// </summary>
        [JsonPropertyName("extension")]
        public string Extension { get; set; } = string.Empty;

        /// <summary>
        /// File creation date
        /// </summary>
        [JsonPropertyName("$createdAt")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// File update date
        /// </summary>
        [JsonPropertyName("$updatedAt")]
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// View URL for the file
        /// </summary>
        [JsonPropertyName("viewUrl")]
        public string ViewUrl { get; set; } = string.Empty;

        /// <summary>
        /// Secure URL (same as ViewUrl for compatibility)
        /// </summary>
        public Uri? SecureUrl { get; set; }

        /// <summary>
        /// URL (same as SecureUrl for compatibility)
        /// </summary>
        public Uri? Url { get; set; }

        /// <summary>
        /// File format (extension without dot)
        /// </summary>
        public string Format { get; set; } = string.Empty;

        /// <summary>
        /// File size in bytes (alias for SizeOriginal)
        /// </summary>
        public long Bytes { get; set; }

        /// <summary>
        /// Initialize Appwrite result and map to compatible fields
        /// </summary>
        public void MapToCompatibleFields()
        {
            // Map Appwrite fields to compatible fields
            PublicId = FileId;
            Bytes = SizeOriginal;
            Format = Extension;

            // Set URLs if ViewUrl is available
            if (!string.IsNullOrEmpty(ViewUrl))
            {
                SecureUrl = new Uri(ViewUrl);
                Url = SecureUrl;
            }
        }
    }

    /// <summary>
    /// Appwrite video upload result
    /// </summary>
    public class AppwriteVideoUploadResult
    {
        /// <summary>
        /// Appwrite file ID
        /// </summary>
        [JsonPropertyName("fileId")]
        public string FileId { get; set; } = string.Empty;

        /// <summary>
        /// Public ID for compatibility
        /// </summary>
        [JsonPropertyName("publicId")]
        public string PublicId { get; set; } = string.Empty;

        /// <summary>
        /// Original file name
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Bucket ID where file is stored
        /// </summary>
        [JsonPropertyName("bucketId")]
        public string BucketId { get; set; } = string.Empty;

        /// <summary>
        /// File MIME type
        /// </summary>
        [JsonPropertyName("mimeType")]
        public string MimeType { get; set; } = string.Empty;

        /// <summary>
        /// File size in bytes
        /// </summary>
        [JsonPropertyName("sizeOriginal")]
        public long SizeOriginal { get; set; }

        /// <summary>
        /// File extension
        /// </summary>
        [JsonPropertyName("extension")]
        public string Extension { get; set; } = string.Empty;

        /// <summary>
        /// File creation date
        /// </summary>
        [JsonPropertyName("$createdAt")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// File update date
        /// </summary>
        [JsonPropertyName("$updatedAt")]
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// View URL for the file
        /// </summary>
        [JsonPropertyName("viewUrl")]
        public string ViewUrl { get; set; } = string.Empty;

        /// <summary>
        /// Secure URL (same as ViewUrl for compatibility)
        /// </summary>
        public Uri? SecureUrl { get; set; }

        /// <summary>
        /// URL (same as SecureUrl for compatibility)
        /// </summary>
        public Uri? Url { get; set; }

        /// <summary>
        /// File format (extension without dot)
        /// </summary>
        public string Format { get; set; } = string.Empty;

        /// <summary>
        /// File size in bytes (alias for SizeOriginal)
        /// </summary>
        public long Bytes { get; set; }

        /// <summary>
        /// Initialize Appwrite result and map to compatible fields
        /// </summary>
        public void MapToCompatibleFields()
        {
            // Map Appwrite fields to compatible fields
            PublicId = FileId;
            Bytes = SizeOriginal;
            Format = System.IO.Path.GetExtension(Name).TrimStart('.');

            // Set URLs if ViewUrl is available
            if (!string.IsNullOrEmpty(ViewUrl))
            {
                SecureUrl = new Uri(ViewUrl);
                Url = SecureUrl;
            }
        }
    }

    /// <summary>
    /// Appwrite raw file upload result
    /// </summary>
    public class AppwriteRawUploadResult
    {
        /// <summary>
        /// Appwrite file ID
        /// </summary>
        [JsonPropertyName("fileId")]
        public string FileId { get; set; } = string.Empty;

        /// <summary>
        /// Public ID for compatibility
        /// </summary>
        [JsonPropertyName("publicId")]
        public string PublicId { get; set; } = string.Empty;

        /// <summary>
        /// Original file name
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Bucket ID where file is stored
        /// </summary>
        [JsonPropertyName("bucketId")]
        public string BucketId { get; set; } = string.Empty;

        /// <summary>
        /// File MIME type
        /// </summary>
        [JsonPropertyName("mimeType")]
        public string MimeType { get; set; } = string.Empty;

        /// <summary>
        /// File size in bytes
        /// </summary>
        [JsonPropertyName("sizeOriginal")]
        public long SizeOriginal { get; set; }

        /// <summary>
        /// File extension
        /// </summary>
        [JsonPropertyName("extension")]
        public string Extension { get; set; } = string.Empty;

        /// <summary>
        /// File creation date
        /// </summary>
        [JsonPropertyName("$createdAt")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// File update date
        /// </summary>
        [JsonPropertyName("$updatedAt")]
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// View URL for the file
        /// </summary>
        [JsonPropertyName("viewUrl")]
        public string ViewUrl { get; set; } = string.Empty;

        /// <summary>
        /// Secure URL (same as ViewUrl for compatibility)
        /// </summary>
        public Uri? SecureUrl { get; set; }

        /// <summary>
        /// URL (same as SecureUrl for compatibility)
        /// </summary>
        public Uri? Url { get; set; }

        /// <summary>
        /// File format (extension without dot)
        /// </summary>
        public string Format { get; set; } = string.Empty;

        /// <summary>
        /// File size in bytes (alias for SizeOriginal)
        /// </summary>
        public long Bytes { get; set; }

        /// <summary>
        /// Initialize Appwrite result and map to compatible fields
        /// </summary>
        public void MapToCompatibleFields()
        {
            // Map Appwrite fields to compatible fields
            PublicId = FileId;
            Bytes = SizeOriginal;
            Format = System.IO.Path.GetExtension(Name).TrimStart('.');

            // Set URLs if ViewUrl is available
            if (!string.IsNullOrEmpty(ViewUrl))
            {
                SecureUrl = new Uri(ViewUrl);
                Url = SecureUrl;
            }
        }
    }

    /// <summary>
    /// Appwrite file deletion result
    /// </summary>
    public class AppwriteDeletionResult
    {
        /// <summary>
        /// Result status
        /// </summary>
        [JsonPropertyName("result")]
        public string Result { get; set; } = string.Empty;

        /// <summary>
        /// Whether deletion was successful
        /// </summary>
        public bool IsSuccess => Result == "success";

        /// <summary>
        /// Error message if deletion failed
        /// </summary>
        [JsonPropertyName("error")]
        public string? Error { get; set; }
    }
}
