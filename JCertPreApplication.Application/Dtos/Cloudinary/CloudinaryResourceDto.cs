using CloudinaryDotNet.Actions;

namespace JCertPreApplication.Application.Dtos.Cloudinary
{
    /// <summary>
    /// DTO đại diện cho một resource trên Cloudinary
    /// </summary>
    public class CloudinaryResourceDto
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
        /// Chuyển đổi từ Cloudinary Resource sang DTO
        /// </summary>
        /// <param name="resource">Cloudinary Resource object</param>
        /// <returns>CloudinaryResourceDto</returns>
        public static CloudinaryResourceDto FromCloudinaryResource(Resource resource)
        {
            return new CloudinaryResourceDto
            {
                PublicId = resource.PublicId ?? string.Empty,
                SecureUrl = resource.SecureUrl?.ToString() ?? string.Empty,
                ResourceType = resource.ResourceType?.ToString() ?? string.Empty,
                Format = resource.Format ?? string.Empty, // Đảm bảo format luôn có cho tất cả resource types
                Bytes = resource.Bytes,
                Width = resource.Width,
                Height = resource.Height,
                CreatedAt = ParseCreatedAt(resource.CreatedAt),
                Folder = ExtractFolderFromPublicId(resource.PublicId),
                Tags = resource.Tags?.ToList() ?? new List<string>(),
                Version = int.TryParse(resource.Version, out var version) ? version : 1
            };
        }

        /// <summary>
        /// Parse Created date string to DateTime
        /// </summary>
        /// <param name="created">Created date string from Cloudinary</param>
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
    }
} 