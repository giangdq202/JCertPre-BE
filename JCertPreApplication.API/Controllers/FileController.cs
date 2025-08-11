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
            
            if (!result.Success)
            {
                return BadRequest(new
                {
                    success = false,
                    message = result.ErrorMessage ?? "Image upload failed"
                });
            }
            
            return Ok(new
            {
                success = true,
                message = "Image uploaded successfully",
                data = new
                {
                    publicId = result.PublicId,
                    url = result.SecureUrl ?? result.Url,
                    format = result.Format,
                    bytes = result.Bytes,
                    createdAt = result.CreatedAt,
                    metadata = result.Metadata
                }
            });
        }

        /// <summary>
        /// Uploads a video or audio file using chunked upload for large files.
        /// </summary>
        /// <param name="file">The video or audio file to upload (MP4, AVI, MOV, WMV, FLV, WebM, MKV, 3GP, MP3, WAV, AAC, OGG, FLAC, M4A, WMA, AMR).</param>
        /// <returns>Upload result with public ID and URL.</returns>
        [HttpPost("upload/video")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadVideo(IFormFile file)
        {
            var result = await _fileService.UploadVideoAsync(file);
            
            if (!result.Success)
            {
                return BadRequest(new
                {
                    success = false,
                    message = result.ErrorMessage ?? "Video upload failed"
                });
            }
            
            var fileType = file.ContentType?.StartsWith("audio/") == true ? "Audio" : "Video";
            return Ok(new
            {
                success = true,
                message = $"{fileType} uploaded successfully",
                data = new
                {
                    publicId = result.PublicId,
                    url = result.SecureUrl ?? result.Url,
                    format = result.Format,
                    bytes = result.Bytes,
                    createdAt = result.CreatedAt,
                    metadata = result.Metadata
                }
            });
        }

        /// <summary>
        /// Uploads a document file.
        /// </summary>
        /// <param name="file">The document file to upload.</param>
        /// <returns>Upload result with public ID and URL.</returns>
        [HttpPost("upload/document")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadDocument(IFormFile file)
        {
            var result = await _fileService.UploadDocumentAsync(file);
            
            if (!result.Success)
            {
                return BadRequest(new
                {
                    success = false,
                    message = result.ErrorMessage ?? "Document upload failed"
                });
            }
            
            return Ok(new
            {
                success = true,
                message = "Document uploaded successfully",
                data = new
                {
                    publicId = result.PublicId,
                    url = result.SecureUrl ?? result.Url,
                    format = result.Format,
                    bytes = result.Bytes,
                    createdAt = result.CreatedAt,
                    metadata = result.Metadata
                }
            });
        }

        /// <summary>
        /// Deletes a file by public ID.
        /// </summary>
        /// <param name="request">Delete request containing the public ID.</param>
        /// <returns>Deletion result.</returns>
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteFile([FromBody] DeleteResourceDto request)
        {
            var result = await _fileService.DeleteFileAsync(request.PublicId);
            
            if (!result.Success)
            {
                return BadRequest(new
                {
                    success = false,
                    message = result.ErrorMessage ?? "File deletion failed"
                });
            }
            
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
        /// Deletes a file by its URL. Automatically extracts the public ID from the URL.
        /// </summary>
        /// <param name="request">Delete request containing the file URL.</param>
        /// <returns>Deletion result.</returns>
        [HttpDelete("delete/by-url")]
        public async Task<IActionResult> DeleteFileByUrl([FromBody] DeleteResourceByUrlDto request)
        {
            var result = await _fileService.DeleteFileByUrlAsync(request.FileUrl);
            
            if (!result.Success)
            {
                return BadRequest(new
                {
                    success = false,
                    message = result.ErrorMessage ?? "File deletion by URL failed"
                });
            }
            
            return Ok(new
            {
                success = true,
                message = "File deleted successfully",
                data = new
                {
                    fileUrl = request.FileUrl,
                    publicId = result.PublicId,
                    result = result.Result
                }
            });
        }

        /// <summary>
        /// Extracts the public ID from a file URL for testing purposes.
        /// </summary>
        /// <param name="request">Request containing the file URL.</param>
        /// <returns>Extracted public ID.</returns>
        [HttpPost("extract-public-id")]
        public IActionResult ExtractPublicId([FromBody] ExtractPublicIdDto request)
        {
            var publicId = _fileService.ExtractPublicIdFromUrl(request.FileUrl);
            
            if (string.IsNullOrEmpty(publicId))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Could not extract public ID from the provided URL"
                });
            }
            
            return Ok(new
            {
                success = true,
                message = "Public ID extracted successfully",
                data = new
                {
                    fileUrl = request.FileUrl,
                    publicId = publicId
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
