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

        public async Task<CloudinaryResourcesResponseDto> GetAllResourcesAsync()
        {
            _logger.LogInformation("Starting to retrieve all resources from Cloudinary...");
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var allResources = new List<CloudinaryResourceDto>();

                // Lặp qua từng loại resource: Image, Video, Raw
                var resourceTypes = new[] { ResourceType.Image, ResourceType.Video, ResourceType.Raw };

                foreach (var resourceType in resourceTypes)
                {
                    _logger.LogDebug("Retrieving resources of type: {ResourceType}", resourceType);
                    
                    // Tạo tham số cho yêu cầu API - chỉ lấy page đầu tiên để test
                    var listParams = new ListResourcesParams()
                    {
                        ResourceType = resourceType,
                        MaxResults = 100, // Giảm số lượng để test
                        Tags = true,
                        Context = true
                    };

                    // Gọi API
                    ListResourcesResult result;
                    try
                    {
                        result = await _cloudinary.ListResourcesAsync(listParams);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error retrieving resources of type {ResourceType}", resourceType);
                        throw ApiException.InternalServerError(
                            "CLOUDINARY_LIST_FAILED", 
                            $"Failed to retrieve {resourceType} resources from Cloudinary: {ex.Message}"
                        );
                    }

                    // Check for Cloudinary-specific errors
                    if (result.Error != null)
                    {
                        _logger.LogError("Cloudinary list resources failed for type {ResourceType}: {ErrorMessage}", resourceType, result.Error.Message);
                        throw ApiException.BadRequest("CLOUDINARY_LIST_FAILED", $"Failed to list {resourceType} resources: {result.Error.Message}");
                    }

                    if (result.Resources != null && result.Resources.Any())
                    {
                        _logger.LogDebug("Found {Count} resources of type {ResourceType}", result.Resources.Count(), resourceType);
                        
                        // Chuyển đổi Cloudinary Resources sang DTOs
                        foreach (var resource in result.Resources)
                        {
                            try
                            {
                                var dto = CloudinaryResourceDto.FromCloudinaryResource(resource);
                                allResources.Add(dto);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogWarning(ex, "Failed to convert resource {PublicId} to DTO", resource.PublicId);
                                // Continue with other resources
                            }
                        }
                    }

                    _logger.LogDebug("Completed retrieving resources of type: {ResourceType}", resourceType);
                }

                stopwatch.Stop();

                // Tạo summary đơn giản
                var summary = new ResourceSummaryDto
                {
                    Images = new ResourceTypeStatDto { Count = allResources.Count(r => r.ResourceType.Equals("image", StringComparison.OrdinalIgnoreCase)) },
                    Videos = new ResourceTypeStatDto { Count = allResources.Count(r => r.ResourceType.Equals("video", StringComparison.OrdinalIgnoreCase)) },
                    RawFiles = new ResourceTypeStatDto { Count = allResources.Count(r => r.ResourceType.Equals("raw", StringComparison.OrdinalIgnoreCase)) }
                };

                var response = new CloudinaryResourcesResponseDto
                {
                    Resources = allResources,
                    Summary = summary,
                    TotalResources = allResources.Count,
                    TotalBytes = allResources.Sum(r => r.Bytes),
                    RetrievedAt = DateTime.UtcNow,
                    ProcessingTimeMs = stopwatch.ElapsedMilliseconds
                };

                _logger.LogInformation("Successfully retrieved {TotalResources} resources from Cloudinary in {ProcessingTimeMs}ms", 
                    response.TotalResources, response.ProcessingTimeMs);

                return response;
            }
            catch (ApiException)
            {
                stopwatch.Stop();
                throw; // Re-throw ApiException as-is
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Unexpected error while retrieving all resources from Cloudinary");
                throw ApiException.InternalServerError("GET_ALL_RESOURCES_ERROR", "Đã xảy ra lỗi trong quá trình lấy danh sách resources.");
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