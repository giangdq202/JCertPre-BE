using Appwrite;
using Appwrite.Models;
using Appwrite.Services;
using CloudinaryDotNet.Actions;
using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.File;
using JCertPreApplication.Application.Dtos.File.Appwrite;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Domain.Configuration;
using JCertPreApplication.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;

namespace JCertPreApplication.Persistence.Services.File
{
    /// <summary>
    /// Appwrite implementation of IFileService that provides compatibility with existing Cloudinary-based interface
    /// </summary>
    public class AppwriteFileService : IFileService
    {
        private readonly Client _client;
        private readonly Storage _storage;
        private readonly AppwriteConfiguration _config;
        private readonly ILogger<AppwriteFileService> _logger;

        // Supported file types mapping
        private readonly Dictionary<string, FileType> _mimeTypeMapping = new()
        {
            // Images
            { "image/jpeg", FileType.Image },
            { "image/jpg", FileType.Image },
            { "image/png", FileType.Image },
            { "image/gif", FileType.Image },
            { "image/bmp", FileType.Image },
            { "image/webp", FileType.Image },
            { "image/svg+xml", FileType.Image },

            // Videos
            { "video/mp4", FileType.Video },
            { "video/avi", FileType.Video },
            { "video/mov", FileType.Video },
            { "video/wmv", FileType.Video },
            { "video/flv", FileType.Video },
            { "video/webm", FileType.Video },
            { "video/mkv", FileType.Video },
            { "video/3gp", FileType.Video },

            // Documents
            { "application/pdf", FileType.Document },
            { "application/msword", FileType.Document },
            { "application/vnd.openxmlformats-officedocument.wordprocessingml.document", FileType.Document },
            { "application/vnd.ms-excel", FileType.Document },
            { "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", FileType.Document },
            { "application/vnd.ms-powerpoint", FileType.Document },
            { "application/vnd.openxmlformats-officedocument.presentationml.presentation", FileType.Document },
            { "text/plain", FileType.Document },
            { "application/rtf", FileType.Document }
        };

        public AppwriteFileService(IOptions<AppwriteConfiguration> appwriteConfig, ILogger<AppwriteFileService> logger)
        {
            _logger = logger;
            _config = appwriteConfig.Value;

            // Validate configuration
            _config.Validate();

            try
            {
                // Initialize Appwrite client
                _client = new Client()
                    .SetEndpoint(_config.Endpoint)
                    .SetProject(_config.ProjectId)
                    .SetKey(_config.ApiKey);

                _storage = new Storage(_client);

                _logger.LogInformation("Appwrite File service initialized successfully for project: {ProjectId}", _config.ProjectId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize Appwrite File service");
                throw ApiException.InternalServerError(
                    "APPWRITE_SERVICE_INIT_FAILED", 
                    "Failed to initialize Appwrite File service. Please check your Appwrite configuration."
                );
            }
        }

        /// <summary>
        /// Uploads an image file to Appwrite and returns Cloudinary-compatible result
        /// </summary>
        public async Task<ImageUploadResult> UploadImageAsync(IFormFile file)
        {
            ValidateImageFile(file);

            try
            {
                using var stream = file.OpenReadStream();
                var fileBytes = new byte[stream.Length];
                await stream.ReadAsync(fileBytes, 0, (int)stream.Length);

                var inputFile = InputFile.FromBytes(fileBytes, file.FileName, file.ContentType);
                
                var appwriteFile = await _storage.CreateFile(
                    bucketId: _config.ImagesBucketId,
                    fileId: ID.Unique(),
                    file: inputFile,
                    permissions: new List<string> { 
                        Permission.Read(Appwrite.Role.Any()),  // Allow everyone to read (ViewURL works)
                        Permission.Write(Appwrite.Role.Users()) // Only logged in users can upload
                    }
                );

                var result = MapToImageUploadResult(appwriteFile, FileType.Image);
                
                _logger.LogInformation("Image uploaded successfully to Appwrite. FileId: {FileId}, Size: {Size}KB", 
                    result.PublicId, result.Bytes / 1024);

                return result;
            }
            catch (AppwriteException ex)
            {
                _logger.LogError(ex, "Failed to upload image to Appwrite: {Message}", ex.Message);
                throw ApiException.InternalServerError("APPWRITE_UPLOAD_FAILED", $"Failed to upload image: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during image upload");
                throw ApiException.InternalServerError("IMAGE_UPLOAD_ERROR", "An unexpected error occurred during image upload");
            }
        }

        /// <summary>
        /// Uploads a video file to Appwrite and returns Cloudinary-compatible result
        /// </summary>
        public async Task<VideoUploadResult> UploadVideoAsync(IFormFile file)
        {
            ValidateVideoFile(file);

            try
            {
                using var stream = file.OpenReadStream();
                var fileBytes = new byte[stream.Length];
                await stream.ReadAsync(fileBytes, 0, (int)stream.Length);

                var inputFile = InputFile.FromBytes(fileBytes, file.FileName, file.ContentType);
                
                var appwriteFile = await _storage.CreateFile(
                    bucketId: _config.VideosBucketId,
                    fileId: ID.Unique(),
                    file: inputFile,
                    permissions: new List<string> { 
                        Permission.Read(Appwrite.Role.Any()),  // Allow everyone to read (ViewURL works)
                        Permission.Write(Appwrite.Role.Users()) // Only logged in users can upload
                    }
                );

                var result = MapToVideoUploadResult(appwriteFile, FileType.Video);
                
                _logger.LogInformation("Video uploaded successfully to Appwrite. FileId: {FileId}, Size: {Size}MB", 
                    result.PublicId, result.Bytes / (1024 * 1024));

                return result;
            }
            catch (AppwriteException ex)
            {
                _logger.LogError(ex, "Failed to upload video to Appwrite: {Message}", ex.Message);
                throw ApiException.InternalServerError("APPWRITE_UPLOAD_FAILED", $"Failed to upload video: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during video upload");
                throw ApiException.InternalServerError("VIDEO_UPLOAD_ERROR", "An unexpected error occurred during video upload");
            }
        }

        /// <summary>
        /// Uploads a raw/document file to Appwrite and returns Cloudinary-compatible result
        /// </summary>
        public async Task<RawUploadResult> UploadRawFileAsync(IFormFile file)
        {
            ValidateRawFile(file);

            try
            {
                using var stream = file.OpenReadStream();
                var fileBytes = new byte[stream.Length];
                await stream.ReadAsync(fileBytes, 0, (int)stream.Length);

                var inputFile = InputFile.FromBytes(fileBytes, file.FileName, file.ContentType);
                
                var appwriteFile = await _storage.CreateFile(
                    bucketId: _config.DocumentsBucketId,
                    fileId: ID.Unique(),
                    file: inputFile,
                    permissions: new List<string> { 
                        Permission.Read(Appwrite.Role.Any()),  // Allow everyone to read (ViewURL works)
                        Permission.Write(Appwrite.Role.Users()) // Only logged in users can upload
                    }
                );

                var result = MapToRawUploadResult(appwriteFile, FileType.Document);
                
                _logger.LogInformation("Document uploaded successfully to Appwrite. FileId: {FileId}, Size: {Size}KB", 
                    result.PublicId, result.Bytes / 1024);

                return result;
            }
            catch (AppwriteException ex)
            {
                _logger.LogError(ex, "Failed to upload document to Appwrite: {Message}", ex.Message);
                throw ApiException.InternalServerError("APPWRITE_UPLOAD_FAILED", $"Failed to upload document: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during document upload");
                throw ApiException.InternalServerError("RAW_UPLOAD_ERROR", "An unexpected error occurred during document upload");
            }
        }

        /// <summary>
        /// Deletes an image file from Appwrite
        /// </summary>
        public async Task<DeletionResult> DeleteImageAsync(string publicId)
        {
            return await DeleteFileAsync(publicId, _config.ImagesBucketId, "image");
        }

        /// <summary>
        /// Deletes a video file from Appwrite
        /// </summary>
        public async Task<DeletionResult> DeleteVideoAsync(string publicId)
        {
            return await DeleteFileAsync(publicId, _config.VideosBucketId, "video");
        }

        /// <summary>
        /// Deletes a raw/document file from Appwrite
        /// </summary>
        public async Task<DeletionResult> DeleteRawFileAsync(string publicId)
        {
            return await DeleteFileAsync(publicId, _config.DocumentsBucketId, "document");
        }

        /// <summary>
        /// Gets a page of resources from Appwrite with cursor-based pagination
        /// </summary>
        public async Task<FileResourcesPageDto> GetResourcesPageAsync(int maxResults = 100, string? nextCursor = null, string resourceType = "image")
        {
            try
            {
                var bucketId = resourceType.ToLower() switch
                {
                    "image" => _config.ImagesBucketId,
                    "video" => _config.VideosBucketId,
                    "raw" => _config.DocumentsBucketId,
                    _ => _config.ImagesBucketId
                };

                // Appwrite doesn't use cursor-based pagination like Cloudinary, so we'll use offset/limit
                var offset = 0;
                if (!string.IsNullOrEmpty(nextCursor) && int.TryParse(nextCursor, out var parsedOffset))
                {
                    offset = parsedOffset;
                }

                var filesList = await _storage.ListFiles(
                    bucketId: bucketId,
                    queries: new List<string> 
                    { 
                        Query.Limit(maxResults),
                        Query.Offset(offset)
                    }
                );

                var resources = filesList.Files.Select(f => MapToFileResourceDto(f, resourceType)).ToList();
                
                var hasNextPage = filesList.Total > offset + maxResults;
                var nextCursorValue = hasNextPage ? (offset + maxResults).ToString() : null;

                return new FileResourcesPageDto
                {
                    Resources = resources,
                    NextCursor = nextCursorValue,
                    MaxResults = maxResults,
                    ResourceType = resourceType,
                    RetrievedAt = DateTime.UtcNow
                };
            }
            catch (AppwriteException ex)
            {
                _logger.LogError(ex, "Failed to get resources page from Appwrite: {Message}", ex.Message);
                throw ApiException.InternalServerError("APPWRITE_LIST_FAILED", $"Failed to get resources: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during resources retrieval");
                throw ApiException.InternalServerError("RESOURCES_RETRIEVAL_ERROR", "An unexpected error occurred during resources retrieval");
            }
        }

        #region Private Helper Methods

        private async Task<DeletionResult> DeleteFileAsync(string fileId, string bucketId, string fileType)
        {
            try
            {
                await _storage.DeleteFile(bucketId, fileId);

                _logger.LogInformation("{FileType} deleted successfully from Appwrite. FileId: {FileId}", 
                    fileType, fileId);

                var result = new AppwriteDeletionResult
                {
                    Success = true,
                    FileId = fileId
                };
                result.MapToCloudinaryFields();
                
                return result;
            }
            catch (AppwriteException ex) when (ex.Code == 404)
            {
                _logger.LogWarning("File not found for deletion. FileId: {FileId}", fileId);
                
                var result = new AppwriteDeletionResult
                {
                    Success = false,
                    FileId = fileId,
                    Error = "File not found"
                };
                result.MapToCloudinaryFields();
                
                return result;
            }
            catch (AppwriteException ex)
            {
                _logger.LogError(ex, "Failed to delete {FileType} from Appwrite: {Message}", fileType, ex.Message);
                throw ApiException.InternalServerError("APPWRITE_DELETE_FAILED", $"Failed to delete {fileType}: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during {FileType} deletion", fileType);
                throw ApiException.InternalServerError($"{fileType.ToUpper()}_DELETE_ERROR", $"An unexpected error occurred during {fileType} deletion");
            }
        }

        private ImageUploadResult MapToImageUploadResult(Appwrite.Models.File appwriteFile, FileType fileType)
        {
            var result = new AppwriteImageUploadResult
            {
                FileId = appwriteFile.Id,
                Name = appwriteFile.Name,
                BucketId = appwriteFile.BucketId,
                MimeType = appwriteFile.MimeType,
                SizeOriginal = appwriteFile.SizeOriginal,
                CreatedAt = DateTime.Parse(appwriteFile.CreatedAt),
                UpdatedAt = DateTime.Parse(appwriteFile.UpdatedAt),
                ViewUrl = GetFileViewUrl(appwriteFile.BucketId, appwriteFile.Id),
                FileType = fileType.ToString().ToLower(),
                Extension = Path.GetExtension(appwriteFile.Name).TrimStart('.'),
                SizeFormatted = FormatFileSize(appwriteFile.SizeOriginal),
            };

            result.MapToCloudinaryFields();
            return result;
        }

        private VideoUploadResult MapToVideoUploadResult(Appwrite.Models.File appwriteFile, FileType fileType)
        {
            var result = new AppwriteVideoUploadResult
            {
                FileId = appwriteFile.Id,
                Name = appwriteFile.Name,
                BucketId = appwriteFile.BucketId,
                MimeType = appwriteFile.MimeType,
                SizeOriginal = appwriteFile.SizeOriginal,
                CreatedAt = DateTime.Parse(appwriteFile.CreatedAt),
                UpdatedAt = DateTime.Parse(appwriteFile.UpdatedAt),
                ViewUrl = GetFileViewUrl(appwriteFile.BucketId, appwriteFile.Id),
                FileType = fileType.ToString().ToLower()
            };

            result.MapToCloudinaryFields();
            return result;
        }

        private RawUploadResult MapToRawUploadResult(Appwrite.Models.File appwriteFile, FileType fileType)
        {
            var result = new AppwriteRawUploadResult
            {
                FileId = appwriteFile.Id,
                Name = appwriteFile.Name,
                BucketId = appwriteFile.BucketId,
                MimeType = appwriteFile.MimeType,
                SizeOriginal = appwriteFile.SizeOriginal,
                CreatedAt = DateTime.Parse(appwriteFile.CreatedAt),
                UpdatedAt = DateTime.Parse(appwriteFile.UpdatedAt),
                ViewUrl = GetFileViewUrl(appwriteFile.BucketId, appwriteFile.Id),
                DownloadUrl = GetFileDownloadUrl(appwriteFile.BucketId, appwriteFile.Id),
                FileType = fileType.ToString().ToLower()
            };

            result.MapToCloudinaryFields();
            return result;
        }

        private FileResourceDto MapToFileResourceDto(Appwrite.Models.File appwriteFile, string resourceType)
        {
            return FileResourceDto.FromAppwriteFile(
                appwriteFile,
                GetFileViewUrl(appwriteFile.BucketId, appwriteFile.Id),
                resourceType
            );
        }

        private string GetFileViewUrl(string bucketId, string fileId)
        {
            return $"{_config.Endpoint}/storage/buckets/{bucketId}/files/{fileId}/view?project={_config.ProjectId}";
        }

        private string GetFileDownloadUrl(string bucketId, string fileId)
        {
            return $"{_config.Endpoint}/storage/buckets/{bucketId}/files/{fileId}/download?project={_config.ProjectId}";
        }

        private static string FormatFileSize(long bytes)
        {
            string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
            int counter = 0;
            decimal number = bytes;
            while (Math.Round(number / 1024) >= 1)
            {
                number /= 1024;
                counter++;
            }
            return $"{number:n1} {suffixes[counter]}";
        }

        private void ValidateImageFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("Image validation failed: File is null or empty");
                throw ApiException.BadRequest("INVALID_FILE", "File is required and cannot be empty");
            }

            _logger.LogInformation("Validating image file: {FileName}, Size: {FileSize}MB ({FileSizeBytes} bytes), ContentType: {ContentType}", 
                file.FileName, 
                Math.Round((double)file.Length / (1024 * 1024), 2), 
                file.Length, 
                file.ContentType);

            if (!_mimeTypeMapping.ContainsKey(file.ContentType) || _mimeTypeMapping[file.ContentType] != FileType.Image)
            {
                _logger.LogWarning("Image validation failed: Invalid content type {ContentType} for file {FileName}", file.ContentType, file.FileName);
                throw ApiException.BadRequest("INVALID_FILE_TYPE", $"File type {file.ContentType} is not supported for images");
            }

            var maxSizeBytes = _config.MaxFileSizeMB * 1024 * 1024;
            _logger.LogInformation("Image file size check: File={FileSize}MB, MaxAllowed={MaxSize}MB (Config MaxFileSizeMB={ConfigValue})", 
                Math.Round((double)file.Length / (1024 * 1024), 2), 
                _config.MaxFileSizeMB, 
                _config.MaxFileSizeMB);
                
            if (file.Length > maxSizeBytes)
            {
                _logger.LogWarning("Image validation failed: File size {FileSize}MB exceeds maximum {MaxSize}MB", 
                    Math.Round((double)file.Length / (1024 * 1024), 2), _config.MaxFileSizeMB);
                throw ApiException.BadRequest("FILE_TOO_LARGE", $"File size exceeds maximum allowed size of {_config.MaxFileSizeMB}MB");
            }

            _logger.LogInformation("Image file validation successful: {FileName}", file.FileName);
        }

        private void ValidateVideoFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("Video validation failed: File is null or empty");
                throw ApiException.BadRequest("INVALID_FILE", "File is required and cannot be empty");
            }

            _logger.LogInformation("Validating video file: {FileName}, Size: {FileSize}MB ({FileSizeBytes} bytes), ContentType: {ContentType}", 
                file.FileName, 
                Math.Round((double)file.Length / (1024 * 1024), 2), 
                file.Length, 
                file.ContentType);

            if (!_mimeTypeMapping.ContainsKey(file.ContentType) || _mimeTypeMapping[file.ContentType] != FileType.Video)
            {
                _logger.LogWarning("Video validation failed: Invalid content type {ContentType} for file {FileName}", file.ContentType, file.FileName);
                throw ApiException.BadRequest("INVALID_FILE_TYPE", $"File type {file.ContentType} is not supported for videos");
            }

            // Use configuration limit for videos (sync with Appwrite Console settings)
            var maxSizeBytes = _config.MaxFileSizeMB * 1024 * 1024;
            _logger.LogInformation("Video file size check: File={FileSize}MB, MaxAllowed={MaxSize}MB (Config MaxFileSizeMB={ConfigValue})", 
                Math.Round((double)file.Length / (1024 * 1024), 2), 
                _config.MaxFileSizeMB, 
                _config.MaxFileSizeMB);
                
            if (file.Length > maxSizeBytes)
            {
                _logger.LogWarning("Video validation failed: File size {FileSize}MB exceeds maximum {MaxSize}MB", 
                    Math.Round((double)file.Length / (1024 * 1024), 2), _config.MaxFileSizeMB);
                throw ApiException.BadRequest("FILE_TOO_LARGE", $"File size exceeds maximum allowed size of {_config.MaxFileSizeMB}MB for videos");
            }

            _logger.LogInformation("Video file validation successful: {FileName}", file.FileName);
        }

        private void ValidateRawFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("Document validation failed: File is null or empty");
                throw ApiException.BadRequest("INVALID_FILE", "File is required and cannot be empty");
            }

            _logger.LogInformation("Validating document file: {FileName}, Size: {FileSize}MB ({FileSizeBytes} bytes), ContentType: {ContentType}", 
                file.FileName, 
                Math.Round((double)file.Length / (1024 * 1024), 2), 
                file.Length, 
                file.ContentType);

            if (!_mimeTypeMapping.ContainsKey(file.ContentType) || _mimeTypeMapping[file.ContentType] != FileType.Document)
            {
                _logger.LogWarning("Document validation failed: Invalid content type {ContentType} for file {FileName}", file.ContentType, file.FileName);
                throw ApiException.BadRequest("INVALID_FILE_TYPE", $"File type {file.ContentType} is not supported for documents");
            }

            var maxSizeBytes = _config.MaxFileSizeMB * 1024 * 1024;
            _logger.LogInformation("Document file size check: File={FileSize}MB, MaxAllowed={MaxSize}MB (Config MaxFileSizeMB={ConfigValue})", 
                Math.Round((double)file.Length / (1024 * 1024), 2), 
                _config.MaxFileSizeMB, 
                _config.MaxFileSizeMB);
                
            if (file.Length > maxSizeBytes)
            {
                _logger.LogWarning("Document validation failed: File size {FileSize}MB exceeds maximum {MaxSize}MB", 
                    Math.Round((double)file.Length / (1024 * 1024), 2), _config.MaxFileSizeMB);
                throw ApiException.BadRequest("FILE_TOO_LARGE", $"File size exceeds maximum allowed size of {_config.MaxFileSizeMB}MB");
            }

            _logger.LogInformation("Document file validation successful: {FileName}", file.FileName);
        }

        #endregion
    }
}
