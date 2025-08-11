namespace JCertPreApplication.Application.Dtos.File
{
    /// <summary>
    /// DTO đại diện cho một resource file
    /// </summary>
    public class FileResourceDto
    {
        /// <summary>
        /// Public ID của resource
        /// </summary>
        public string PublicId { get; set; } = string.Empty;

        /// <summary>
        /// URL an toàn (HTTPS) của resource
        /// </summary>
        public string SecureUrl { get; set; } = string.Empty;

        /// <summary>
        /// Loại resource (image, video, raw)
        /// </summary>
        public string ResourceType { get; set; } = string.Empty;

        /// <summary>
        /// Định dạng file (jpg, mp4, pdf, v.v.)
        /// </summary>
        public string Format { get; set; } = string.Empty;

        /// <summary>
        /// Kích thước file tính bằng bytes
        /// </summary>
        public long Bytes { get; set; }

        /// <summary>
        /// Chiều rộng (cho image/video)
        /// </summary>
        public int? Width { get; set; }

        /// <summary>
        /// Chiều cao (cho image/video)
        /// </summary>
        public int? Height { get; set; }

        /// <summary>
        /// Ngày tạo resource
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Folder chứa resource
        /// </summary>
        public string? Folder { get; set; }

        /// <summary>
        /// Tags của resource
        /// </summary>
        public List<string> Tags { get; set; } = new List<string>();

        /// <summary>
        /// Version của resource
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Creates a FileResourceDto from Appwrite file data
        /// </summary>
        /// <param name="appwriteFile">The Appwrite file object</param>
        /// <param name="viewUrl">The view URL for the file</param>
        /// <param name="resourceType">The resource type (image, video, raw)</param>
        /// <returns>FileResourceDto</returns>
        public static FileResourceDto FromAppwriteFile(object appwriteFile, string viewUrl, string resourceType)
        {
            // Since we're dealing with dynamic Appwrite.Models.File object, we need to use reflection
            var fileType = appwriteFile.GetType();
            
            var getId = fileType.GetProperty("Id")?.GetValue(appwriteFile)?.ToString() ?? "";
            var getName = fileType.GetProperty("Name")?.GetValue(appwriteFile)?.ToString() ?? "";
            var getMimeType = fileType.GetProperty("MimeType")?.GetValue(appwriteFile)?.ToString() ?? "";
            var getSizeOriginal = fileType.GetProperty("SizeOriginal")?.GetValue(appwriteFile);
            var getCreatedAt = fileType.GetProperty("CreatedAt")?.GetValue(appwriteFile)?.ToString() ?? "";

            var sizeOriginal = getSizeOriginal is long size ? size : 0L;
            
            return new FileResourceDto
            {
                PublicId = getId,
                SecureUrl = viewUrl,
                ResourceType = resourceType,
                Format = Path.GetExtension(getName).TrimStart('.'),
                Bytes = sizeOriginal,
                CreatedAt = DateTime.TryParse(getCreatedAt, out var created) ? created : DateTime.UtcNow,
                Folder = null, // Appwrite doesn't have folder concept
                Tags = new List<string>(), // Appwrite doesn't have tags
                Version = 1 // Default version for Appwrite files
            };
        }

        /// <summary>
        /// Parse Created date string to DateTime
        /// </summary>
        /// <param name="created">Created date string</param>
        /// <returns>DateTime</returns>
        private static DateTime ParseCreatedAt(string created)
        {
            if (DateTime.TryParse(created, out var dateTime))
            {
                return dateTime;
            }
            return DateTime.UtcNow; // Fallback to current time
        }

        /// <summary>
        /// Extract folder from public ID
        /// </summary>
        /// <param name="publicId">Public ID that may contain folder path</param>
        /// <returns>Folder path or null</returns>
        private static string? ExtractFolderFromPublicId(string? publicId)
        {
            if (string.IsNullOrEmpty(publicId))
                return null;

            var lastSlashIndex = publicId.LastIndexOf('/');
            if (lastSlashIndex > 0)
            {
                return publicId[..lastSlashIndex];
            }
            return null;
        }

        /// <summary>
        /// Format file size for display
        /// </summary>
        /// <param name="bytes">File size in bytes</param>
        /// <returns>Formatted file size string</returns>
        private static string FormatFileSize(long bytes)
        {
            string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
            int counter = 0;
            decimal number = bytes;
            while (Math.Round(number / 1024) >= 1)
            {
                number /= 1024;
                counter++;
            }
            return $"{number:n1} {suffixes[counter]}";
        }
    }
}
