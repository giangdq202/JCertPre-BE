namespace JCertPreApplication.Application.Dtos.Cloudinary
{
    /// <summary>
    /// DTO đại diện cho response danh sách resources từ Cloudinary
    /// </summary>
    public class CloudinaryResourcesResponseDto
    {
        /// <summary>
        /// Danh sách tất cả resources
        /// </summary>
        public List<CloudinaryResourceDto> Resources { get; set; } = new List<CloudinaryResourceDto>();

        /// <summary>
        /// Summary thống kê theo từng loại resource
        /// </summary>
        public ResourceSummaryDto Summary { get; set; } = new ResourceSummaryDto();

        /// <summary>
        /// Tổng số resources
        /// </summary>
        public int TotalResources { get; set; }

        /// <summary>
        /// Tổng dung lượng (bytes)
        /// </summary>
        public long TotalBytes { get; set; }

        /// <summary>
        /// Thời gian lấy dữ liệu
        /// </summary>
        public DateTime RetrievedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Thời gian xử lý (milliseconds)
        /// </summary>
        public long ProcessingTimeMs { get; set; }
    }

    /// <summary>
    /// DTO thống kê summary theo loại resource
    /// </summary>
    public class ResourceSummaryDto
    {
        /// <summary>
        /// Thống kê image resources
        /// </summary>
        public ResourceTypeStatDto Images { get; set; } = new ResourceTypeStatDto();

        /// <summary>
        /// Thống kê video resources
        /// </summary>
        public ResourceTypeStatDto Videos { get; set; } = new ResourceTypeStatDto();

        /// <summary>
        /// Thống kê raw file resources
        /// </summary>
        public ResourceTypeStatDto RawFiles { get; set; } = new ResourceTypeStatDto();
    }

    /// <summary>
    /// DTO thống kê cho một loại resource
    /// </summary>
    public class ResourceTypeStatDto
    {
        /// <summary>
        /// Số lượng files
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Tổng dung lượng (bytes)
        /// </summary>
        public long TotalBytes { get; set; }

        /// <summary>
        /// Dung lượng trung bình (bytes)
        /// </summary>
        public double AverageBytes => Count > 0 ? (double)TotalBytes / Count : 0;

        /// <summary>
        /// Định dạng files phổ biến nhất
        /// </summary>
        public List<string> PopularFormats { get; set; } = new List<string>();

        /// <summary>
        /// Resource mới nhất
        /// </summary>
        public DateTime? LatestCreatedAt { get; set; }

        /// <summary>
        /// Resource cũ nhất
        /// </summary>
        public DateTime? OldestCreatedAt { get; set; }
    }
} 