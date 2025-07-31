using CloudinaryDotNet.Actions;
using System.Text.Json.Serialization;

namespace JCertPreApplication.Application.Dtos.File.Appwrite
{
    /// <summary>
    /// Appwrite file upload response that mimics Cloudinary's ImageUploadResult
    /// </summary>
    public class AppwriteImageUploadResult : ImageUploadResult
    {
        /// <summary>
        /// Appwrite file ID (mapped to PublicId for compatibility)
        /// </summary>
        [JsonPropertyName("fileId")]
        public string FileId { get; set; } = string.Empty;

        /// <summary>
        /// Original file name
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Bucket ID where the file is stored
        /// </summary>
        [JsonPropertyName("bucketId")]
        public string BucketId { get; set; } = string.Empty;

        /// <summary>
        /// MIME type of the file
        /// </summary>
        [JsonPropertyName("mimeType")]
        public string MimeType { get; set; } = string.Empty;

        /// <summary>
        /// File size in bytes
        /// </summary>
        [JsonPropertyName("sizeOriginal")]
        public long SizeOriginal { get; set; }

        /// <summary>
        /// File creation timestamp
        /// </summary>
        [JsonPropertyName("createdAt")]
        public new DateTime CreatedAt { get; set; }

        /// <summary>
        /// File last update timestamp
        /// </summary>
        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// View URL for the file
        /// </summary>
        [JsonPropertyName("viewUrl")]
        public string ViewUrl { get; set; } = string.Empty;

        /// <summary>
        /// Download URL for the file (when applicable)
        /// </summary>
        [JsonPropertyName("downloadUrl")]
        public string? DownloadUrl { get; set; }

        /// <summary>
        /// Formatted file size for display
        /// </summary>
        [JsonPropertyName("sizeFormatted")]
        public string SizeFormatted { get; set; } = string.Empty;

        /// <summary>
        /// File extension
        /// </summary>
        [JsonPropertyName("extension")]
        public string Extension { get; set; } = string.Empty;

        /// <summary>
        /// File type (image, video, document)
        /// </summary>
        [JsonPropertyName("fileType")]
        public string FileType { get; set; } = string.Empty;

        /// <summary>
        /// Initialize Appwrite result and map to Cloudinary-compatible fields
        /// </summary>
        public void MapToCloudinaryFields()
        {
            // Map Appwrite fields to Cloudinary-compatible fields
            PublicId = FileId;
            Bytes = SizeOriginal;
            Format = Extension;
            
            // Create URLs
            if (!string.IsNullOrEmpty(ViewUrl))
            {
                SecureUrl = new Uri(ViewUrl);
                Url = SecureUrl;
            }

            // Set creation time
            base.CreatedAt = CreatedAt;
        }
    }

    /// <summary>
    /// Appwrite video upload result that mimics Cloudinary's VideoUploadResult
    /// </summary>
    public class AppwriteVideoUploadResult : VideoUploadResult
    {
        /// <summary>
        /// Appwrite file ID (mapped to PublicId for compatibility)
        /// </summary>
        [JsonPropertyName("fileId")]
        public string FileId { get; set; } = string.Empty;

        /// <summary>
        /// Original file name
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Bucket ID where the file is stored
        /// </summary>
        [JsonPropertyName("bucketId")]
        public string BucketId { get; set; } = string.Empty;

        /// <summary>
        /// MIME type of the file
        /// </summary>
        [JsonPropertyName("mimeType")]
        public string MimeType { get; set; } = string.Empty;

        /// <summary>
        /// File size in bytes
        /// </summary>
        [JsonPropertyName("sizeOriginal")]
        public long SizeOriginal { get; set; }

        /// <summary>
        /// File creation timestamp
        /// </summary>
        [JsonPropertyName("createdAt")]
        public new DateTime CreatedAt { get; set; }

        /// <summary>
        /// File last update timestamp
        /// </summary>
        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// View URL for the file
        /// </summary>
        [JsonPropertyName("viewUrl")]
        public string ViewUrl { get; set; } = string.Empty;

        /// <summary>
        /// File type (image, video, document)
        /// </summary>
        [JsonPropertyName("fileType")]
        public string FileType { get; set; } = string.Empty;

        /// <summary>
        /// Initialize Appwrite result and map to Cloudinary-compatible fields
        /// </summary>
        public void MapToCloudinaryFields()
        {
            // Map Appwrite fields to Cloudinary-compatible fields
            PublicId = FileId;
            Bytes = SizeOriginal;
            Format = System.IO.Path.GetExtension(Name).TrimStart('.');
            
            // Create URLs
            if (!string.IsNullOrEmpty(ViewUrl))
            {
                SecureUrl = new Uri(ViewUrl);
                Url = SecureUrl;
            }

            // Set creation time
            base.CreatedAt = CreatedAt;
        }
    }

    /// <summary>
    /// Appwrite raw file upload result that mimics Cloudinary's RawUploadResult
    /// </summary>
    public class AppwriteRawUploadResult : RawUploadResult
    {
        /// <summary>
        /// Appwrite file ID (mapped to PublicId for compatibility)
        /// </summary>
        [JsonPropertyName("fileId")]
        public string FileId { get; set; } = string.Empty;

        /// <summary>
        /// Original file name
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Bucket ID where the file is stored
        /// </summary>
        [JsonPropertyName("bucketId")]
        public string BucketId { get; set; } = string.Empty;

        /// <summary>
        /// MIME type of the file
        /// </summary>
        [JsonPropertyName("mimeType")]
        public string MimeType { get; set; } = string.Empty;

        /// <summary>
        /// File size in bytes
        /// </summary>
        [JsonPropertyName("sizeOriginal")]
        public long SizeOriginal { get; set; }

        /// <summary>
        /// File creation timestamp
        /// </summary>
        [JsonPropertyName("createdAt")]
        public new DateTime CreatedAt { get; set; }

        /// <summary>
        /// File last update timestamp
        /// </summary>
        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// View URL for the file
        /// </summary>
        [JsonPropertyName("viewUrl")]
        public string ViewUrl { get; set; } = string.Empty;

        /// <summary>
        /// Download URL for the file
        /// </summary>
        [JsonPropertyName("downloadUrl")]
        public string? DownloadUrl { get; set; }

        /// <summary>
        /// File type (image, video, document)
        /// </summary>
        [JsonPropertyName("fileType")]
        public string FileType { get; set; } = string.Empty;

        /// <summary>
        /// Initialize Appwrite result and map to Cloudinary-compatible fields
        /// </summary>
        public void MapToCloudinaryFields()
        {
            // Map Appwrite fields to Cloudinary-compatible fields
            PublicId = FileId;
            Bytes = SizeOriginal;
            Format = System.IO.Path.GetExtension(Name).TrimStart('.');
            
            // Create URLs
            if (!string.IsNullOrEmpty(ViewUrl))
            {
                SecureUrl = new Uri(ViewUrl);
                Url = SecureUrl;
            }

            // Set creation time
            base.CreatedAt = CreatedAt;
        }
    }

    /// <summary>
    /// Appwrite deletion result that mimics Cloudinary's DeletionResult
    /// </summary>
    public class AppwriteDeletionResult : DeletionResult
    {
        /// <summary>
        /// Whether the deletion was successful
        /// </summary>
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        /// <summary>
        /// File ID that was deleted
        /// </summary>
        [JsonPropertyName("fileId")]
        public string FileId { get; set; } = string.Empty;

        /// <summary>
        /// Error message if deletion failed
        /// </summary>
        [JsonPropertyName("error")]
        public new string? Error { get; set; }

        /// <summary>
        /// Initialize Appwrite result and map to Cloudinary-compatible fields
        /// </summary>
        public void MapToCloudinaryFields()
        {
            // Map success to Cloudinary's Result field
            Result = Success ? "ok" : "not found";
        }
    }
}
