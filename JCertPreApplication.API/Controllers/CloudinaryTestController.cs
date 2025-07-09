using JCertPreApplication.Application.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace JCertPreApplication.API.Controllers
{
    /// <summary>
    /// Test controller for CloudinaryService functionality.
    /// Used for testing file upload and management operations.
    /// </summary>
    [Route("api/cloudinary-test")]
    [ApiController]
    [Tags("CloudinaryTest")]
    [Produces("application/json")]
    public class CloudinaryTestController : ControllerBase
    {
        private readonly ICloudinaryService _cloudinaryService;

        public CloudinaryTestController(ICloudinaryService cloudinaryService)
        {
            _cloudinaryService = cloudinaryService ?? throw new ArgumentNullException(nameof(cloudinaryService));
        }

        /// <summary>
        /// Test image upload to Cloudinary.
        /// Accepts common image formats (JPEG, PNG, GIF, WebP, etc.)
        /// </summary>
        /// <param name="imageFile">Image file to upload (max 5MB recommended)</param>
        /// <returns>Upload result with URL and public ID</returns>
        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                return BadRequest(new { error = "Vui lòng chọn một tệp hình ảnh để tải lên." });
            }

            var result = await _cloudinaryService.UploadImageAsync(imageFile);

            if (result.Error != null)
            {
                return BadRequest(new 
                { 
                    error = "Upload failed",
                    message = result.Error.Message
                });
            }

            return Ok(new
            {
                message = "Upload hình ảnh thành công!",
                imageUrl = result.SecureUrl.ToString(),
                publicId = result.PublicId,
                format = result.Format,
                width = result.Width,
                height = result.Height,
                bytes = result.Bytes,
                createdAt = result.CreatedAt
            });
        }

        /// <summary>
        /// Test video upload to Cloudinary.
        /// Uses UploadLarge method for better handling of large video files.
        /// </summary>
        /// <param name="videoFile">Video file to upload</param>
        /// <returns>Upload result with URL and public ID</returns>
        [HttpPost("upload-video")]
        public async Task<IActionResult> UploadVideo(IFormFile videoFile)
        {
            if (videoFile == null || videoFile.Length == 0)
            {
                return BadRequest(new { error = "Vui lòng chọn một tệp video để tải lên." });
            }

            var result = await _cloudinaryService.UploadVideoAsync(videoFile);

            if (result.Error != null)
            {
                return BadRequest(new 
                { 
                    error = "Upload failed",
                    message = result.Error.Message
                });
            }

            return Ok(new
            {
                message = "Upload video thành công!",
                videoUrl = result.SecureUrl.ToString(),
                publicId = result.PublicId,
                format = result.Format,
                duration = result.Duration,
                width = result.Width,
                height = result.Height,
                bytes = result.Bytes,
                createdAt = result.CreatedAt
            });
        }

        /// <summary>
        /// Test raw file upload to Cloudinary.
        /// For documents, PDFs, archives, and other non-media files.
        /// </summary>
        /// <param name="rawFile">Raw file to upload</param>
        /// <returns>Upload result with URL and public ID</returns>
        [HttpPost("upload-document")]
        public async Task<IActionResult> UploadRawFile(IFormFile rawFile)
        {
            if (rawFile == null || rawFile.Length == 0)
            {
                return BadRequest(new { error = "Vui lòng chọn một tệp để tải lên." });
            }

            var result = await _cloudinaryService.UploadRawFileAsync(rawFile);

            if (result.Error != null)
            {
                return BadRequest(new 
                { 
                    error = "Upload failed",
                    message = result.Error.Message
                });
            }

            return Ok(new
            {
                message = "Upload tệp thành công!",
                fileUrl = result.SecureUrl.ToString(),
                publicId = result.PublicId,
                format = result.Format,
                bytes = result.Bytes,
                createdAt = result.CreatedAt
            });
        }

        /// <summary>
        /// Test image deletion from Cloudinary.
        /// </summary>
        /// <param name="publicId">Public ID of the image to delete</param>
        /// <returns>Deletion result</returns>
        [HttpDelete("delete-image/{publicId}")]
        public async Task<IActionResult> DeleteImage(string publicId)
        {
            if (string.IsNullOrEmpty(publicId))
            {
                return BadRequest(new { error = "Public ID là bắt buộc." });
            }

            var result = await _cloudinaryService.DeleteImageAsync(publicId);

            if (result.Error != null)
            {
                return BadRequest(new 
                { 
                    error = "Delete failed",
                    message = result.Error.Message
                });
            }

            return Ok(new
            {
                message = "Xóa hình ảnh thành công!",
                publicId = publicId,
                result = result.Result
            });
        }

        /// <summary>
        /// Test video deletion from Cloudinary.
        /// </summary>
        /// <param name="publicId">Public ID of the video to delete</param>
        /// <returns>Deletion result</returns>
        [HttpDelete("delete-video/{publicId}")]
        public async Task<IActionResult> DeleteVideo(string publicId)
        {
            if (string.IsNullOrEmpty(publicId))
            {
                return BadRequest(new { error = "Public ID là bắt buộc." });
            }

            var result = await _cloudinaryService.DeleteVideoAsync(publicId);

            if (result.Error != null)
            {
                return BadRequest(new 
                { 
                    error = "Delete failed",
                    message = result.Error.Message
                });
            }

            return Ok(new
            {
                message = "Xóa video thành công!",
                publicId = publicId,
                result = result.Result
            });
        }

        /// <summary>
        /// Test raw file deletion from Cloudinary.
        /// </summary>
        /// <param name="publicId">Public ID of the raw file to delete</param>
        /// <returns>Deletion result</returns>
        [HttpDelete("delete-document/{publicId}")]
        public async Task<IActionResult> DeleteRawFile(string publicId)
        {
            if (string.IsNullOrEmpty(publicId))
            {
                return BadRequest(new { error = "Public ID là bắt buộc." });
            }

            var result = await _cloudinaryService.DeleteRawFileAsync(publicId);

            if (result.Error != null)
            {
                return BadRequest(new 
                { 
                    error = "Delete failed",
                    message = result.Error.Message
                });
            }

            return Ok(new
            {
                message = "Xóa tệp thành công!",
                publicId = publicId,
                result = result.Result
            });
        }

        /// <summary>
        /// Get CloudinaryService health status.
        /// </summary>
        /// <returns>Service status information</returns>
        [HttpGet("health")]
        public IActionResult GetHealthStatus()
        {
            return Ok(new
            {
                service = "CloudinaryService",
                status = "OK",
                message = "CloudinaryService đã sẵn sàng để sử dụng!",
                availableEndpoints = new[]
                {
                    "POST /api/cloudinary-test/upload-image",
                    "POST /api/cloudinary-test/upload-video", 
                    "POST /api/cloudinary-test/upload-document",
                    "DELETE /api/cloudinary-test/delete-image/{publicId}",
                    "DELETE /api/cloudinary-test/delete-video/{publicId}",
                    "DELETE /api/cloudinary-test/delete-document/{publicId}"
                }
            });
        }
    }
} 