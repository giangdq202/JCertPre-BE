using CloudinaryDotNet.Actions;
using JCertPreApplication.Application.Dtos.Cloudinary;
using Microsoft.AspNetCore.Http;

namespace JCertPreApplication.Application.Contracts
{
    public interface ICloudinaryService
    {
        /// <summary>
        /// Tải lên một tệp hình ảnh.
        /// </summary>
        /// <param name="file">Tệp từ request.</param>
        /// <returns>Kết quả tải lên từ Cloudinary.</returns>
        Task<ImageUploadResult> UploadImageAsync(IFormFile file);

        /// <summary>
        /// Tải lên một tệp video. Sẽ tự động dùng phương thức UploadLarge.
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
        /// Xóa một tệp video từ Cloudinary bằng public ID.
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
        /// Lấy tất cả các resource từ Cloudinary (Images, Videos, Raw files).
        /// Xử lý phân trang tự động và gộp kết quả từ tất cả các loại resource.
        /// </summary>
        /// <returns>Danh sách tất cả resources với thông tin thống kê</returns>
        Task<CloudinaryResourcesResponseDto> GetAllResourcesAsync();
    }
} 