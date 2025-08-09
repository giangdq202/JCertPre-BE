using CloudinaryDotNet.Actions;
using JCertPreApplication.Application.Dtos.File;
using Microsoft.AspNetCore.Http;

namespace JCertPreApplication.Application.Contracts
{
    public interface IFileService
    {
        /// <summary>
        /// Tải lên một tệp hình ảnh.
        /// </summary>
        /// <param name="file">Tệp từ request.</param>
        /// <returns>Kết quả tải lên từ Cloudinary.</returns>
        Task<ImageUploadResult> UploadImageAsync(IFormFile file);

        /// <summary>
        /// Tải lên một tệp video hoặc audio. Sẽ tự động dùng phương thức UploadLarge.
        /// </summary>
        /// <param name="file">Tệp từ request.</param>
        /// <returns>Kết quả tải lên từ Cloudinary.</returns>
        Task<VideoUploadResult> UploadVideoAsync(IFormFile file);

        /// <summary>
        /// Tải lên một tệp thô (document, zip, v.v.).
        /// </summary>
        /// <param name="file">Tệp từ request.</param>
        /// <returns>Kết quả tải lên từ Cloudinary.</returns>
        Task<RawUploadResult> UploadRawFileAsync(IFormFile file);

        /// <summary>
        /// Xóa một tệp hình ảnh từ Cloudinary bằng public ID.
        /// </summary>
        /// <param name="publicId">Public ID của tệp cần xóa.</param>
        /// <returns>Kết quả xóa từ Cloudinary.</returns>
        Task<DeletionResult> DeleteImageAsync(string publicId);

        /// <summary>
        /// Xóa một tệp video hoặc audio từ Cloudinary bằng public ID.
        /// </summary>
        /// <param name="publicId">Public ID của tệp cần xóa.</param>
        /// <returns>Kết quả xóa từ Cloudinary.</returns>
        Task<DeletionResult> DeleteVideoAsync(string publicId);

        /// <summary>
        /// Xóa một tệp raw từ Cloudinary bằng public ID.
        /// </summary>
        /// <param name="publicId">Public ID của tệp cần xóa.</param>
        /// <returns>Kết quả xóa từ Cloudinary.</returns>
        Task<DeletionResult> DeleteRawFileAsync(string publicId);

        /// <summary>
        /// Lấy một trang resources từ Cloudinary với hỗ trợ cursor-based pagination.
        /// Cho phép client yêu cầu các trang tùy ý và nhận cursor cho trang tiếp theo.
        /// </summary>
        /// <param name="maxResults">Số lượng tối đa items mỗi trang (1-500)</param>
        /// <param name="nextCursor">Cursor trả về từ trang trước; null cho trang đầu tiên</param>
        /// <param name="resourceType">Loại resource cần lọc: "image" (mặc định), "video", "audio", hoặc "raw"</param>
        /// <returns>Một trang resources và cursor cho trang tiếp theo (nếu có)</returns>
        Task<FileResourcesPageDto> GetResourcesPageAsync(int maxResults = 100, string? nextCursor = null, string resourceType = "image");
    }
}
