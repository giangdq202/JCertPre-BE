using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.File;
using Microsoft.AspNetCore.Mvc;

namespace JCertPreApplication.API.Controllers
{
    /// <summary>
    /// Handles file operations including upload, delete, and resource management.
    /// </summary>
    [Route("api/files")]
    [ApiController]
    [Tags("Files")]
    [Produces("application/json")]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;

        public FileController(IFileService fileService)
        {
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
        }

        /// <summary>
        /// Uploads an image file.
        /// </summary>
        /// <param name="file">The image file to upload (JPEG, PNG, GIF, BMP, WebP, SVG).</param>
        /// <returns>Upload result with public ID and URL.</returns>
        [HttpPost("upload/image")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            var result = await _fileService.UploadImageAsync(file);
            return Ok(new
            {
                success = true,
                message = "Image uploaded successfully",
                data = new
                {
                    publicId = result.PublicId,
                    url = result.SecureUrl?.ToString() ?? result.Url?.ToString(),
                    width = result.Width,
                    height = result.Height,
                    format = result.Format,
                    bytes = result.Bytes,
                    createdAt = result.CreatedAt
                }
            });
        }

        /// <summary>
        /// Uploads a video file using chunked upload for large files.
        /// </summary>
        /// <param name="file">The video file to upload (MP4, AVI, MOV, WMV, FLV, WebM, MKV, 3GP).</param>
        /// <returns>Upload result with public ID and URL.</returns>
        [HttpPost("upload/video")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadVideo(IFormFile file)
        {
            var result = await _fileService.UploadVideoAsync(file);
            return Ok(new
            {
                success = true,
                message = "Video uploaded successfully",
                data = new
                {
                    publicId = result.PublicId,
                    url = result.SecureUrl?.ToString() ?? result.Url?.ToString(),
                    width = result.Width,
                    height = result.Height,
                    format = result.Format,
                    duration = result.Duration,
                    bytes = result.Bytes,
                    createdAt = result.CreatedAt
                }
            });
        }

        /// <summary>
        /// Uploads a raw file (documents, archives, etc.).
        /// </summary>
        /// <param name="file">The raw file to upload.</param>
        /// <returns>Upload result with public ID and URL.</returns>
        [HttpPost("upload/file")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadRawFile(IFormFile file)
        {
            var result = await _fileService.UploadRawFileAsync(file);
            return Ok(new
            {
                success = true,
                message = "File uploaded successfully",
                data = new
                {
                    publicId = result.PublicId,
                    url = result.SecureUrl?.ToString() ?? result.Url?.ToString(),
                    format = result.Format,
                    bytes = result.Bytes,
                    createdAt = result.CreatedAt
                }
            });
        }

        /// <summary>
        /// Deletes an image by public ID.
        /// </summary>
        /// <param name="request">Delete request containing the public ID.</param>
        /// <returns>Deletion result.</returns>
        [HttpDelete("delete/image")]
        public async Task<IActionResult> DeleteImage([FromBody] DeleteResourceDto request)
        {
            var result = await _fileService.DeleteImageAsync(request.PublicId);
            return Ok(new
            {
                success = true,
                message = "Image deleted successfully",
                data = new
                {
                    publicId = request.PublicId,
                    result = result.Result
                }
            });
        }

        /// <summary>
        /// Deletes a video by public ID.
        /// </summary>
        /// <param name="request">Delete request containing the public ID.</param>
        /// <returns>Deletion result.</returns>
        [HttpDelete("delete/video")]
        public async Task<IActionResult> DeleteVideo([FromBody] DeleteResourceDto request)
        {
            var result = await _fileService.DeleteVideoAsync(request.PublicId);
            return Ok(new
            {
                success = true,
                message = "Video deleted successfully",
                data = new
                {
                    publicId = request.PublicId,
                    result = result.Result
                }
            });
        }

        /// <summary>
        /// Deletes a raw file by public ID.
        /// </summary>
        /// <param name="request">Delete request containing the public ID.</param>
        /// <returns>Deletion result.</returns>
        [HttpDelete("delete/file")]
        public async Task<IActionResult> DeleteRawFile([FromBody] DeleteResourceDto request)
        {
            var result = await _fileService.DeleteRawFileAsync(request.PublicId);
            return Ok(new
            {
                success = true,
                message = "File deleted successfully",
                data = new
                {
                    publicId = request.PublicId,
                    result = result.Result
                }
            });
        }

        /// <summary>
        /// Gets a paginated list of resources.
        /// </summary>
        /// <param name="maxResults">Maximum number of results per page (1-500). Default: 100.</param>
        /// <param name="nextCursor">Cursor for the next page. Use null for the first page.</param>
        /// <param name="resourceType">Type of resources to retrieve: "image", "video", or "raw". Default: "image".</param>
        /// <returns>Paginated list of resources with cursor for next page.</returns>
        [HttpGet("resources")]
        public async Task<IActionResult> GetResources(
            [FromQuery] int maxResults = 100,
            [FromQuery] string? nextCursor = null,
            [FromQuery] string resourceType = "image")
        {
            var result = await _fileService.GetResourcesPageAsync(maxResults, nextCursor, resourceType);
            return Ok(new
            {
                success = true,
                message = "Resources retrieved successfully",
                data = result
            });
        }

        /// <summary>
        /// Test endpoint to check if file service is working.
        /// </summary>
        /// <returns>Service status.</returns>
        [HttpGet("health")]
        public IActionResult HealthCheck()
        {
            return Ok(new
            {
                success = true,
                message = "File service is healthy",
                timestamp = DateTime.UtcNow
            });
        }
    }
}
