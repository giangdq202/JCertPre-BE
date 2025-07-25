using Microsoft.AspNetCore.Mvc;
using CloudinaryDotNet.Actions;
using JCertPreApplication.Application.Contracts;

namespace JCertPreApplication.API.Controllers
{
    [Route("api/cloudinary")]
    [ApiController]
    public class CloudinaryController : ControllerBase
    {
        private readonly ICloudinaryService _service;

        public CloudinaryController(ICloudinaryService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        /// <summary>
        /// Upload an image file to Cloudinary.
        /// </summary>
        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile file)
        {
            var result = await _service.UploadImageAsync(file);
            return Ok(new
            {
                result.PublicId,
                url    = result.SecureUrl,
                bytes  = result.Bytes,
                format = result.Format
            });
        }

        /// <summary>
        /// Upload a video file to Cloudinary.
        /// </summary>
        [HttpPost("upload-video")]
        public async Task<IActionResult> UploadVideo([FromForm] IFormFile file)
        {
            var result = await _service.UploadVideoAsync(file);
            return Ok(new
            {
                result.PublicId,
                url      = result.SecureUrl,
                bytes    = result.Bytes,
                duration = result.Duration,
                format   = result.Format
            });
        }

        /// <summary>
        /// Upload a raw/document file to Cloudinary.
        /// </summary>
        [HttpPost("upload-raw")]
        public async Task<IActionResult> UploadRaw([FromForm] IFormFile file)
        {
            var result = await _service.UploadRawFileAsync(file);
            return Ok(new
            {
                result.PublicId,
                url    = result.SecureUrl,
                bytes  = result.Bytes,
                format = result.Format
            });
        }

        /// <summary>
        /// Delete a resource (image, video, raw) from Cloudinary.
        /// </summary>
        [HttpDelete("delete")]
        public async Task<IActionResult> Delete([FromBody] DeleteRequest dto)
        {
            if (dto is null || string.IsNullOrWhiteSpace(dto.Type) || string.IsNullOrWhiteSpace(dto.PublicId))
                return BadRequest(new { error = "Type and PublicId are required." });

            DeletionResult result = dto.Type.ToLowerInvariant() switch
            {
                "image" => await _service.DeleteImageAsync(dto.PublicId),
                "video" => await _service.DeleteVideoAsync(dto.PublicId),
                "raw"   => await _service.DeleteRawFileAsync(dto.PublicId),
                _       => throw new ArgumentException($"Unsupported type '{dto.Type}'.")
            };

            return Ok(new
            {
                dto.PublicId,
                result = result.Result
            });
        }

        /// <summary>
        /// Retrieve all resources from Cloudinary with summary stats.
        /// </summary>
        [HttpGet("resources")]
        public async Task<IActionResult> GetAll()
        {
            var dto = await _service.GetAllResourcesAsync();
            return Ok(new
            {
                total   = dto.TotalResources,
                sizeMB  = Math.Round(dto.TotalBytes / 1024.0 / 1024.0, 2),
                items   = dto.Resources
            });
        }

        /// <summary>
        /// Health check for CloudinaryService.
        /// </summary>
        [HttpGet("health")]
        public IActionResult Health()
            => Ok(new { service = "CloudinaryService", status = "OK" });
    }

    /// <summary>
    /// DTO for delete request body.
    /// </summary>
    public class DeleteRequest
    {
        /// <summary>
        /// Resource type: "image", "video", or "raw".
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Public ID of the resource (e.g. "documents/note_ojmfmp.txt").
        /// </summary>
        public string PublicId { get; set; } = string.Empty;
    }
}
