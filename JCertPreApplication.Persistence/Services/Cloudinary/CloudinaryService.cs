using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.Cloudinary;
using JCertPreApplication.Application.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace JCertPreApplication.Persistence.Services.Cloudinary
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly CloudinaryDotNet.Cloudinary _cloudinary;
        private readonly ILogger<CloudinaryService> _logger;
        private const int UPLOAD_TIMEOUT_SECONDS = 600; // 10 minutes
        private const int CHUNK_SIZE = 20 * 1024 * 1024; // 20MB chunks
        private const int MAX_CONCURRENT_UPLOADS = 4;

        public CloudinaryService(IOptions<JCertPreApplication.Domain.Configuration.CloudinaryConfiguration> cloudinaryConfig, ILogger<CloudinaryService> logger)
        {
            _logger = logger;

            var config = cloudinaryConfig.Value;

            // Validate required configuration
            if (string.IsNullOrEmpty(config.CloudName) || 
                string.IsNullOrEmpty(config.ApiKey) || 
                string.IsNullOrEmpty(config.ApiSecret))
            {
                throw ApiException.InternalServerError(
                    "CLOUDINARY_CONFIG_MISSING", 
                    "CloudName, ApiKey, and ApiSecret are required for Cloudinary configuration."
                );
            }

            try
            {
                // Initialize Cloudinary instance with custom HttpClient
                var account = new Account(config.CloudName, config.ApiKey, config.ApiSecret);
                var httpClient = new HttpClient
                {
                    Timeout = TimeSpan.FromSeconds(UPLOAD_TIMEOUT_SECONDS)
                };
                _cloudinary = new CloudinaryDotNet.Cloudinary(account);
                _cloudinary.Api.Secure = config.Secure;

                _logger.LogInformation("Cloudinary service initialized successfully for cloud: {CloudName}", config.CloudName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize Cloudinary service");
                throw ApiException.InternalServerError(
                    "CLOUDINARY_INIT_FAILED", 
                    "Failed to initialize Cloudinary service. Please check your Cloudinary configuration."
                );
            }
        }

        public async Task<ImageUploadResult> UploadImageAsync(IFormFile file)
        {
            // Validate input
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("Image upload failed: file is null or empty");
                throw ApiException.BadRequest("FILE_REQUIRED", "Không có tệp được cung cấp.");
            }

            // Validate file type
            if (!IsImageFile(file))
            {
                _logger.LogWarning("Image upload failed: invalid file type {ContentType}", file.ContentType);
                throw ApiException.BadRequest("INVALID_FILE_TYPE", "Tệp không phải là hình ảnh hợp lệ.");
            }

            var uploadParams = new ImageUploadParams();
            var stream = file.OpenReadStream();
            uploadParams.File = new FileDescription(file.FileName, stream);
            uploadParams.Folder = "images"; // Organize uploads in folders
            uploadParams.UseFilename = true;
            uploadParams.UniqueFilename = true;

            try
            {
                var result = await _cloudinary.UploadAsync(uploadParams);
                
                // Ensure stream is disposed after upload
                await stream.DisposeAsync();
                
                // Check for Cloudinary-specific errors
                if (result.Error != null)
                {
                    _logger.LogError("Cloudinary image upload failed: {ErrorMessage}", result.Error.Message);
                    throw ApiException.BadRequest("CLOUDINARY_UPLOAD_FAILED", $"Upload failed: {result.Error.Message}");
                }

                _logger.LogDebug("Image uploaded successfully: {PublicId}", result.PublicId);
                return result;
            }
            catch (ApiException)
            {
                await stream.DisposeAsync();
                throw; // Re-throw ApiException as-is
            }
            catch (Exception ex)
            {
                await stream.DisposeAsync();
                _logger.LogError(ex, "Unexpected error during image upload");
                throw ApiException.InternalServerError("IMAGE_UPLOAD_ERROR", "Đã xảy ra lỗi trong quá trình tải lên hình ảnh.");
            }
        }

        public async Task<VideoUploadResult> UploadVideoAsync(IFormFile file)
        {
            // Validate input
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("Video upload failed: file is null or empty");
                throw ApiException.BadRequest("FILE_REQUIRED", "Không có tệp được cung cấp.");
            }

            // Validate file type
            if (!IsVideoFile(file))
            {
                _logger.LogWarning("Video upload failed: invalid file type {ContentType}", file.ContentType);
                throw ApiException.BadRequest("INVALID_FILE_TYPE", "Tệp không phải là video hợp lệ.");
            }

            _logger.LogInformation("Starting video upload: {FileName}, Size: {Size}MB", file.FileName, file.Length / (1024 * 1024));

            var uploadParams = new VideoUploadParams();
            var stream = file.OpenReadStream();
            uploadParams.File = new FileDescription(file.FileName, stream);
            uploadParams.Folder = "videos"; // Organize uploads in folders
            uploadParams.UseFilename = true;
            uploadParams.UniqueFilename = true;

            // Use custom cancellation token with extended timeout
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(UPLOAD_TIMEOUT_SECONDS));
            
            try
            {
                // Use UploadLarge for video files with custom chunk size and concurrent uploads
                var result = await _cloudinary.UploadLargeAsync<VideoUploadResult>(
                    uploadParams,
                    CHUNK_SIZE,
                    MAX_CONCURRENT_UPLOADS,
                    cts.Token
                );
                
                // Ensure stream is disposed after upload
                await stream.DisposeAsync();
                
                // Check for Cloudinary-specific errors
                if (result.Error != null)
                {
                    _logger.LogError("Cloudinary video upload failed: {ErrorMessage}", result.Error.Message);
                    throw ApiException.BadRequest("CLOUDINARY_UPLOAD_FAILED", $"Upload failed: {result.Error.Message}");
                }

                _logger.LogInformation("Video uploaded successfully: {PublicId}, Duration: {Duration}s, Size: {Size}MB", 
                    result.PublicId, 
                    result.Duration,
                    result.Bytes / (1024 * 1024));

                return result;
            }
            catch (TaskCanceledException)
            {
                await stream.DisposeAsync();
                _logger.LogError("Video upload timed out after {Timeout} seconds", UPLOAD_TIMEOUT_SECONDS);
                throw ApiException.BadRequest("UPLOAD_TIMEOUT", $"Tải lên video đã hết thời gian chờ sau {UPLOAD_TIMEOUT_SECONDS} giây.");
            }
            catch (ApiException)
            {
                await stream.DisposeAsync();
                throw; // Re-throw ApiException as-is
            }
            catch (Exception ex)
            {
                await stream.DisposeAsync();
                _logger.LogError(ex, "Unexpected error during video upload");
                throw ApiException.InternalServerError("VIDEO_UPLOAD_ERROR", "Đã xảy ra lỗi trong quá trình tải lên video.");
            }
        }

        public async Task<RawUploadResult> UploadRawFileAsync(IFormFile file)
        {
            // Validate input
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("Raw file upload failed: file is null or empty");
                throw ApiException.BadRequest("FILE_REQUIRED", "Không có tệp được cung cấp.");
            }

            var uploadParams = new RawUploadParams();
            var stream = file.OpenReadStream();
            uploadParams.File = new FileDescription(file.FileName, stream);
            uploadParams.Folder = "documents"; // Organize uploads in folders
            uploadParams.UseFilename = true;
            uploadParams.UniqueFilename = true;

            try
            {
                var result = await _cloudinary.UploadAsync(uploadParams);
                
                // Ensure stream is disposed after upload
                await stream.DisposeAsync();
                
                // Check for Cloudinary-specific errors
                if (result.Error != null)
                {
                    _logger.LogError("Cloudinary raw file upload failed: {ErrorMessage}", result.Error.Message);
                    throw ApiException.BadRequest("CLOUDINARY_UPLOAD_FAILED", $"Upload failed: {result.Error.Message}");
                }

                _logger.LogDebug("Raw file uploaded successfully: {PublicId}", result.PublicId);
                return result;
            }
            catch (ApiException)
            {
                await stream.DisposeAsync();
                throw; // Re-throw ApiException as-is
            }
            catch (Exception ex)
            {
                await stream.DisposeAsync();
                _logger.LogError(ex, "Unexpected error during raw file upload");
                throw ApiException.InternalServerError("RAW_FILE_UPLOAD_ERROR", "Đã xảy ra lỗi trong quá trình tải lên tệp.");
            }
        }

        public async Task<DeletionResult> DeleteImageAsync(string publicId)
        {
            // Validate input
            if (string.IsNullOrEmpty(publicId))
            {
                _logger.LogWarning("Image deletion failed: publicId is null or empty");
                throw ApiException.BadRequest("PUBLIC_ID_REQUIRED", "Public ID không được cung cấp.");
            }

            var deletionParams = new DeletionParams(publicId)
            {
                ResourceType = ResourceType.Image
            };

            try
            {
                var result = await _cloudinary.DestroyAsync(deletionParams);
                
                // Check for Cloudinary-specific errors
                if (result.Error != null)
                {
                    _logger.LogError("Cloudinary image deletion failed: {ErrorMessage}", result.Error.Message);
                    throw ApiException.BadRequest("CLOUDINARY_DELETE_FAILED", $"Delete failed: {result.Error.Message}");
                }

                _logger.LogDebug("Image deleted successfully: {PublicId}", publicId);
                return result;
            }
            catch (ApiException)
            {
                throw; // Re-throw ApiException as-is
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during image deletion for publicId: {PublicId}", publicId);
                throw ApiException.InternalServerError("IMAGE_DELETE_ERROR", "Đã xảy ra lỗi trong quá trình xóa hình ảnh.");
            }
        }

        public async Task<DeletionResult> DeleteVideoAsync(string publicId)
        {
            // Validate input
            if (string.IsNullOrEmpty(publicId))
            {
                _logger.LogWarning("Video deletion failed: publicId is null or empty");
                throw ApiException.BadRequest("PUBLIC_ID_REQUIRED", "Public ID không được cung cấp.");
            }

            var deletionParams = new DeletionParams(publicId)
            {
                ResourceType = ResourceType.Video
            };

            try
            {
                var result = await _cloudinary.DestroyAsync(deletionParams);
                
                // Check for Cloudinary-specific errors
                if (result.Error != null)
                {
                    _logger.LogError("Cloudinary video deletion failed: {ErrorMessage}", result.Error.Message);
                    throw ApiException.BadRequest("CLOUDINARY_DELETE_FAILED", $"Delete failed: {result.Error.Message}");
                }

                _logger.LogDebug("Video deleted successfully: {PublicId}", publicId);
                return result;
            }
            catch (ApiException)
            {
                throw; // Re-throw ApiException as-is
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during video deletion for publicId: {PublicId}", publicId);
                throw ApiException.InternalServerError("VIDEO_DELETE_ERROR", "Đã xảy ra lỗi trong quá trình xóa video.");
            }
        }

        public async Task<DeletionResult> DeleteRawFileAsync(string publicId)
        {
            // Validate input
            if (string.IsNullOrEmpty(publicId))
            {
                _logger.LogWarning("Raw file deletion failed: publicId is null or empty");
                throw ApiException.BadRequest("PUBLIC_ID_REQUIRED", "Public ID không được cung cấp.");
            }

            var deletionParams = new DeletionParams(publicId)
            {
                ResourceType = ResourceType.Raw
            };

            try
            {
                var result = await _cloudinary.DestroyAsync(deletionParams);
                
                // Check for Cloudinary-specific errors
                if (result.Error != null)
                {
                    _logger.LogError("Cloudinary raw file deletion failed: {ErrorMessage}", result.Error.Message);
                    throw ApiException.BadRequest("CLOUDINARY_DELETE_FAILED", $"Delete failed: {result.Error.Message}");
                }

                _logger.LogDebug("Raw file deleted successfully: {PublicId}", publicId);
                return result;
            }
            catch (ApiException)
            {
                throw; // Re-throw ApiException as-is
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during raw file deletion for publicId: {PublicId}", publicId);
                throw ApiException.InternalServerError("RAW_FILE_DELETE_ERROR", "Đã xảy ra lỗi trong quá trình xóa tệp.");
            }
        }

        public async Task<CloudinaryResourcesPageDto> GetResourcesPageAsync(int maxResults = 100, string? nextCursor = null, string resourceType = "image")
        {
            // Validate maxResults parameter
            if (maxResults < 1 || maxResults > 500)
            {
                _logger.LogWarning("Invalid maxResults parameter: {MaxResults}. Must be between 1 and 500", maxResults);
                throw ApiException.BadRequest("INVALID_MAX_RESULTS", "maxResults phải nằm trong khoảng từ 1 đến 500.");
            }

            // Map resourceType string to Cloudinary ResourceType enum
            var resourceTypeEnum = resourceType.ToLowerInvariant() switch
            {
                "image" => ResourceType.Image,
                "video" => ResourceType.Video,
                "raw" => ResourceType.Raw,
                _ => throw ApiException.BadRequest("INVALID_RESOURCE_TYPE", "resourceType phải là image, video hoặc raw.")
            };

            _logger.LogInformation("Retrieving resources: Type={ResourceType}, MaxResults={MaxResults}, NextCursor={NextCursor}",
                resourceType, maxResults, nextCursor ?? "null");

            var stopwatch = Stopwatch.StartNew();

            try
            {
                var allDtos = new List<CloudinaryResourceDto>();

                // Single resource type request
                var result = await GetResourcesForType(resourceTypeEnum, maxResults, nextCursor);
                
                if (result.Resources != null && result.Resources.Any())
                {
                    _logger.LogDebug("Found {Count} resources of type {ResourceType}", result.Resources.Count(), resourceType);
                    
                    // Convert Cloudinary Resources to DTOs (format handling is done in DTO converter)
                    foreach (var resource in result.Resources)
                    {
                        try
                        {
                            var dto = CloudinaryResourceDto.FromCloudinaryResource(resource);
                            allDtos.Add(dto);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Failed to convert resource {PublicId} to DTO", resource.PublicId);
                            // Continue with other resources
                        }
                    }
                }

                stopwatch.Stop();

                var pageDto = new CloudinaryResourcesPageDto
                {
                    Resources = allDtos.AsReadOnly(),
                    NextCursor = result.NextCursor,
                    MaxResults = maxResults,
                    ResourceType = resourceType,
                    RetrievedAt = DateTime.UtcNow,
                    ProcessingTimeMs = stopwatch.ElapsedMilliseconds
                };

                _logger.LogInformation("Successfully retrieved {ActualResults} resources (Type: {ResourceType}) from Cloudinary in {ProcessingTimeMs}ms. HasNextPage: {HasNextPage}", 
                    pageDto.ActualResults, resourceType, pageDto.ProcessingTimeMs, pageDto.HasNextPage);

                return pageDto;
            }
            catch (ApiException)
            {
                stopwatch.Stop();
                throw; // Re-throw ApiException as-is
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Unexpected error while retrieving paginated resources from Cloudinary");
                throw ApiException.InternalServerError("GET_PAGINATED_RESOURCES_ERROR", "Đã xảy ra lỗi trong quá trình lấy danh sách resources phân trang.");
            }
        }

        /// <summary>
        /// Helper method to get resources for a specific type
        /// </summary>
        private async Task<ListResourcesResult> GetResourcesForType(ResourceType resourceType, int maxResults, string? nextCursor)
        {
            var listParams = new ListResourcesParams
            {
                ResourceType = resourceType,
                MaxResults = Math.Min(maxResults, 500), // Ensure we don't exceed Cloudinary limits
                NextCursor = nextCursor,
                Tags = true,
                Context = true
            };

            try
            {
                var result = await _cloudinary.ListResourcesAsync(listParams);
                
                // Check for Cloudinary-specific errors
                if (result.Error != null)
                {
                    _logger.LogError("Cloudinary list failed for type {ResourceType}: {ErrorMessage}", resourceType, result.Error.Message);
                    throw ApiException.BadRequest("CLOUDINARY_LIST_FAILED", $"Failed to list {resourceType} resources: {result.Error.Message}");
                }

                return result;
            }
            catch (Exception ex) when (!(ex is ApiException))
            {
                _logger.LogError(ex, "Error retrieving resources of type {ResourceType}", resourceType);
                throw ApiException.InternalServerError(
                    "CLOUDINARY_LIST_FAILED", 
                    $"Failed to retrieve {resourceType} resources from Cloudinary: {ex.Message}"
                );
            }
        }

        #region Private Helper Methods

        private static bool IsImageFile(IFormFile file)
        {
            var allowedImageTypes = new[]
            {
                "image/jpeg", "image/jpg", "image/png", "image/gif", 
                "image/bmp", "image/webp", "image/svg+xml"
            };
            
            return allowedImageTypes.Contains(file.ContentType?.ToLowerInvariant());
        }

        private static bool IsVideoFile(IFormFile file)
        {
            var allowedVideoTypes = new[]
            {
                "video/mp4", "video/avi", "video/mov", "video/wmv", 
                "video/flv", "video/webm", "video/mkv", "video/3gp"
            };
            
            return allowedVideoTypes.Contains(file.ContentType?.ToLowerInvariant());
        }

        #endregion
    }
} 