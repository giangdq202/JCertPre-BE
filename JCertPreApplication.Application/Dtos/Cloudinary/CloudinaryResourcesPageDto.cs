namespace JCertPreApplication.Application.Dtos.Cloudinary
{
    /// <summary>
    /// DTO đại diện cho một trang kết quả resources từ Cloudinary với hỗ trợ phân trang cursor-based
    /// </summary>
    public class CloudinaryResourcesPageDto
    {
        /// <summary>
        /// Danh sách resources trong trang hiện tại
        /// </summary>
        public IReadOnlyList<CloudinaryResourceDto> Resources { get; init; } = new List<CloudinaryResourceDto>();

        /// <summary>
        /// Cursor để lấy trang tiếp theo (null nếu không có trang tiếp theo)
        /// </summary>
        public string? NextCursor { get; init; }

        /// <summary>
        /// Số lượng tối đa items mỗi trang đã yêu cầu
        /// </summary>
        public int MaxResults { get; init; }

        /// <summary>
        /// Số lượng resources thực tế trả về trong trang này
        /// </summary>
        public int ActualResults => Resources.Count;

        /// <summary>
        /// Có trang tiếp theo hay không
        /// </summary>
        public bool HasNextPage => !string.IsNullOrEmpty(NextCursor);

        /// <summary>
        /// Thời gian lấy dữ liệu
        /// </summary>
        public DateTime RetrievedAt { get; init; } = DateTime.UtcNow;

        /// <summary>
        /// Thời gian xử lý (milliseconds)
        /// </summary>
        public long ProcessingTimeMs { get; init; }
    }
} 