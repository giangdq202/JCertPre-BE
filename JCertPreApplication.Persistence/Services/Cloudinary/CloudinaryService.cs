using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Domain.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http;

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

            try
            {
                var config = cloudinaryConfig.Value;

                // Validate required configuration
                if (string.IsNullOrEmpty(config.CloudName) || 
                    string.IsNullOrEmpty(config.ApiKey) || 
                    string.IsNullOrEmpty(config.ApiSecret))
                {
                    throw new ArgumentException("CloudName, ApiKey, and ApiSecret are required for Cloudinary configuration.");
                }

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
                throw new InvalidOperationException("Failed to initialize Cloudinary service. Please check your Cloudinary configuration.", ex);
            }
        }

        public async Task<ImageUploadResult> UploadImageAsync(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    _logger.LogWarning("Image upload failed: file is null or empty");
                    return new ImageUploadResult 
                    { 
                        Error = new Error { Message = "Không có tệp được cung cấp." } 
                    };
                }

                // Validate file type
                if (!IsImageFile(file))
                {
                    _logger.LogWarning("Image upload failed: invalid file type {ContentType}", file.ContentType);
                    return new ImageUploadResult 
                    { 
                        Error = new Error { Message = "Tệp không phải là hình ảnh hợp lệ." } 
                    };
                }

                var uploadParams = new ImageUploadParams();
                var stream = file.OpenReadStream();
                uploadParams.File = new FileDescription(file.FileName, stream);
                uploadParams.Folder = "images"; // Organize uploads in folders
                uploadParams.UseFilename = true;
                uploadParams.UniqueFilename = true;

                var result = await _cloudinary.UploadAsync(uploadParams);
                
                // Ensure stream is disposed after upload
                await stream.DisposeAsync();
                
                if (result.Error != null)
                {
                    _logger.LogError("Cloudinary image upload failed: {ErrorMessage}", result.Error.Message);
                }
                else
                {
                    _logger.LogDebug("Image uploaded successfully: {PublicId}", result.PublicId);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during image upload");
                return new ImageUploadResult 
                { 
                    Error = new Error { Message = "Đã xảy ra lỗi trong quá trình tải lên hình ảnh." } 
                };
            }
        }

        public async Task<VideoUploadResult> UploadVideoAsync(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    _logger.LogWarning("Video upload failed: file is null or empty");
                    return new VideoUploadResult 
                    { 
                        Error = new Error { Message = "Không có tệp được cung cấp." } 
                    };
                }

                // Validate file type
                if (!IsVideoFile(file))
                {
                    _logger.LogWarning("Video upload failed: invalid file type {ContentType}", file.ContentType);
                    return new VideoUploadResult 
                    { 
                        Error = new Error { Message = "Tệp không phải là video hợp lệ." } 
                    };
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
                
                // Use UploadLarge for video files with custom chunk size and concurrent uploads
                var result = await _cloudinary.UploadLargeAsync<VideoUploadResult>(
                    uploadParams,
                    CHUNK_SIZE,
                    MAX_CONCURRENT_UPLOADS,
                    cts.Token
                );
                
                // Ensure stream is disposed after upload
                await stream.DisposeAsync();
                
                if (result.Error != null)
                {
                    _logger.LogError("Cloudinary video upload failed: {ErrorMessage}", result.Error.Message);
                }
                else
                {
                    _logger.LogInformation("Video uploaded successfully: {PublicId}, Duration: {Duration}s, Size: {Size}MB", 
                        result.PublicId, 
                        result.Duration,
                        result.Bytes / (1024 * 1024));
                }

                return result;
            }
            catch (TaskCanceledException)
            {
                _logger.LogError("Video upload timed out after {Timeout} seconds", UPLOAD_TIMEOUT_SECONDS);
                return new VideoUploadResult 
                { 
                    Error = new Error { Message = $"Tải lên video đã hết thời gian chờ sau {UPLOAD_TIMEOUT_SECONDS} giây." } 
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during video upload");
                return new VideoUploadResult 
                { 
                    Error = new Error { Message = "Đã xảy ra lỗi trong quá trình tải lên video." } 
                };
            }
        }

        public async Task<RawUploadResult> UploadRawFileAsync(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    _logger.LogWarning("Raw file upload failed: file is null or empty");
                    return new RawUploadResult 
                    { 
                        Error = new Error { Message = "Không có tệp được cung cấp." } 
                    };
                }

                var uploadParams = new RawUploadParams();
                var stream = file.OpenReadStream();
                uploadParams.File = new FileDescription(file.FileName, stream);
                uploadParams.Folder = "documents"; // Organize uploads in folders
                uploadParams.UseFilename = true;
                uploadParams.UniqueFilename = true;

                var result = await _cloudinary.UploadAsync(uploadParams);
                
                // Ensure stream is disposed after upload
                await stream.DisposeAsync();
                
                if (result.Error != null)
                {
                    _logger.LogError("Cloudinary raw file upload failed: {ErrorMessage}", result.Error.Message);
                }
                else
                {
                    _logger.LogDebug("Raw file uploaded successfully: {PublicId}", result.PublicId);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during raw file upload");
                return new RawUploadResult 
                { 
                    Error = new Error { Message = "Đã xảy ra lỗi trong quá trình tải lên tệp." } 
                };
            }
        }

        public async Task<DeletionResult> DeleteImageAsync(string publicId)
        {
            try
            {
                if (string.IsNullOrEmpty(publicId))
                {
                    _logger.LogWarning("Image deletion failed: publicId is null or empty");
                    return new DeletionResult 
                    { 
                        Error = new Error { Message = "Public ID không được cung cấp." } 
                    };
                }

                var deletionParams = new DeletionParams(publicId)
                {
                    ResourceType = ResourceType.Image
                };

                var result = await _cloudinary.DestroyAsync(deletionParams);
                
                if (result.Error != null)
                {
                    _logger.LogError("Cloudinary image deletion failed: {ErrorMessage}", result.Error.Message);
                }
                else
                {
                    _logger.LogDebug("Image deleted successfully: {PublicId}", publicId);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during image deletion for publicId: {PublicId}", publicId);
                return new DeletionResult 
                { 
                    Error = new Error { Message = "Đã xảy ra lỗi trong quá trình xóa hình ảnh." } 
                };
            }
        }

        public async Task<DeletionResult> DeleteVideoAsync(string publicId)
        {
            try
            {
                if (string.IsNullOrEmpty(publicId))
                {
                    _logger.LogWarning("Video deletion failed: publicId is null or empty");
                    return new DeletionResult 
                    { 
                        Error = new Error { Message = "Public ID không được cung cấp." } 
                    };
                }

                var deletionParams = new DeletionParams(publicId)
                {
                    ResourceType = ResourceType.Video
                };

                var result = await _cloudinary.DestroyAsync(deletionParams);
                
                if (result.Error != null)
                {
                    _logger.LogError("Cloudinary video deletion failed: {ErrorMessage}", result.Error.Message);
                }
                else
                {
                    _logger.LogDebug("Video deleted successfully: {PublicId}", publicId);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during video deletion for publicId: {PublicId}", publicId);
                return new DeletionResult 
                { 
                    Error = new Error { Message = "Đã xảy ra lỗi trong quá trình xóa video." } 
                };
            }
        }

        public async Task<DeletionResult> DeleteRawFileAsync(string publicId)
        {
            try
            {
                if (string.IsNullOrEmpty(publicId))
                {
                    _logger.LogWarning("Raw file deletion failed: publicId is null or empty");
                    return new DeletionResult 
                    { 
                        Error = new Error { Message = "Public ID không được cung cấp." } 
                    };
                }

                var deletionParams = new DeletionParams(publicId)
                {
                    ResourceType = ResourceType.Raw
                };

                var result = await _cloudinary.DestroyAsync(deletionParams);
                
                if (result.Error != null)
                {
                    _logger.LogError("Cloudinary raw file deletion failed: {ErrorMessage}", result.Error.Message);
                }
                else
                {
                    _logger.LogDebug("Raw file deleted successfully: {PublicId}", publicId);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during raw file deletion for publicId: {PublicId}", publicId);
                return new DeletionResult 
                { 
                    Error = new Error { Message = "Đã xảy ra lỗi trong quá trình xóa tệp." } 
                };
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