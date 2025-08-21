using Appwrite;
using Appwrite.Models;
using Appwrite.Services;
using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.File;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Configuration;
using JCertPreApplication.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JCertPreApplication.Persistence.Services.File
{
    /// <summary>
    /// Appwrite implementation of IFileService providing generic file storage operations
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

            // Audio files (stored in video bucket)
            { "audio/mpeg", FileType.Video },
            { "audio/mp3", FileType.Video },
            { "audio/wav", FileType.Video },
            { "audio/aac", FileType.Video },
            { "audio/ogg", FileType.Video },
            { "audio/opus", FileType.Video },
            { "audio/flac", FileType.Video },
            { "audio/m4a", FileType.Video },
            { "audio/wma", FileType.Video },
            { "audio/amr", FileType.Video },
            { "audio/3gpp", FileType.Video },
            { "audio/webm", FileType.Video },

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
        /// Uploads an image file to Appwrite
        /// </summary>
        public async Task<FileUploadResult> UploadImageAsync(IFormFile file, CancellationToken cancellationToken = default)
        {
            ValidateImageFile(file);

            try
            {
                using var stream = file.OpenReadStream();
                var fileBytes = new byte[stream.Length];
                await stream.ReadAsync(fileBytes, 0, (int)stream.Length, cancellationToken);

                var inputFile = InputFile.FromBytes(fileBytes, file.FileName, file.ContentType);
                
                var appwriteFile = await _storage.CreateFile(
                    bucketId: _config.ImagesBucketId,
                    fileId: ID.Unique(),
                    file: inputFile,
                    permissions: new List<string> { 
                        Permission.Read(Appwrite.Role.Any()),  // Allow everyone to read
                        Permission.Write(Appwrite.Role.Users()) // Only logged in users can upload
                    }
                );

                var result = MapToFileUploadResult(appwriteFile, "image");
                
                _logger.LogInformation("Image uploaded successfully to Appwrite. FileId: {FileId}, Size: {Size}KB", 
                    result.PublicId, result.Bytes / 1024);

                return result;
            }
            catch (AppwriteException ex)
            {
                _logger.LogError(ex, "Failed to upload image to Appwrite: {Message}", ex.Message);
                return new FileUploadResult 
                { 
                    Success = false, 
                    ErrorMessage = $"Failed to upload image: {ex.Message}" 
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during image upload");
                return new FileUploadResult 
                { 
                    Success = false, 
                    ErrorMessage = "An unexpected error occurred during image upload" 
                };
            }
        }

        /// <summary>
        /// Uploads a video or audio file to Appwrite
        /// </summary>
        public async Task<FileUploadResult> UploadVideoAsync(IFormFile file, CancellationToken cancellationToken = default)
        {
            ValidateVideoFile(file);

            try
            {
                using var stream = file.OpenReadStream();
                var fileBytes = new byte[stream.Length];
                await stream.ReadAsync(fileBytes, 0, (int)stream.Length, cancellationToken);

                var inputFile = InputFile.FromBytes(fileBytes, file.FileName, file.ContentType);
                
                var appwriteFile = await _storage.CreateFile(
                    bucketId: _config.VideosBucketId,
                    fileId: ID.Unique(),
                    file: inputFile,
                    permissions: new List<string> { 
                        Permission.Read(Appwrite.Role.Any()),  // Allow everyone to read
                        Permission.Write(Appwrite.Role.Users()) // Only logged in users can upload
                    }
                );

                var result = MapToFileUploadResult(appwriteFile, "video");
                
                var fileType = file.ContentType.StartsWith("audio/") ? "Audio" : "Video";
                _logger.LogInformation("{FileType} uploaded successfully to Appwrite. FileId: {FileId}, Size: {Size}MB", 
                    fileType, result.PublicId, result.Bytes / (1024 * 1024));

                return result;
            }
            catch (AppwriteException ex)
            {
                _logger.LogError(ex, "Failed to upload video/audio to Appwrite: {Message}", ex.Message);
                return new FileUploadResult 
                { 
                    Success = false, 
                    ErrorMessage = $"Failed to upload video/audio: {ex.Message}" 
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during video/audio upload");
                return new FileUploadResult 
                { 
                    Success = false, 
                    ErrorMessage = "An unexpected error occurred during video/audio upload" 
                };
            }
        }

        /// <summary>
        /// Uploads a document file to Appwrite
        /// </summary>
        public async Task<FileUploadResult> UploadDocumentAsync(IFormFile file, CancellationToken cancellationToken = default)
        {
            ValidateDocumentFile(file);

            try
            {
                using var stream = file.OpenReadStream();
                var fileBytes = new byte[stream.Length];
                await stream.ReadAsync(fileBytes, 0, (int)stream.Length, cancellationToken);

                var inputFile = InputFile.FromBytes(fileBytes, file.FileName, file.ContentType);
                
                var appwriteFile = await _storage.CreateFile(
                    bucketId: _config.DocumentsBucketId,
                    fileId: ID.Unique(),
                    file: inputFile,
                    permissions: new List<string> { 
                        Permission.Read(Appwrite.Role.Any()),  // Allow everyone to read
                        Permission.Write(Appwrite.Role.Users()) // Only logged in users can upload
                    }
                );

                var result = MapToFileUploadResult(appwriteFile, "document");
                
                _logger.LogInformation("Document uploaded successfully to Appwrite. FileId: {FileId}, Size: {Size}KB", 
                    result.PublicId, result.Bytes / 1024);

                return result;
            }
            catch (AppwriteException ex)
            {
                _logger.LogError(ex, "Failed to upload document to Appwrite: {Message}", ex.Message);
                return new FileUploadResult 
                { 
                    Success = false, 
                    ErrorMessage = $"Failed to upload document: {ex.Message}" 
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during document upload");
                return new FileUploadResult 
                { 
                    Success = false, 
                    ErrorMessage = "An unexpected error occurred during document upload" 
                };
            }
        }

        /// <summary>
        /// Deletes a file from Appwrite using its public ID
        /// </summary>
        public async Task<FileDeletionResult> DeleteFileAsync(string publicId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(publicId))
            {
                _logger.LogWarning("File deletion failed: publicId is null or empty");
                return new FileDeletionResult 
                { 
                    Success = false, 
                    PublicId = publicId, 
                    ErrorMessage = "Public ID is required" 
                };
            }

            try
            {
                // Try to delete from each bucket until we find the file
                var buckets = new[] 
                { 
                    (_config.ImagesBucketId, "image"),
                    (_config.VideosBucketId, "video"),
                    (_config.DocumentsBucketId, "document")
                };

                foreach (var (bucketId, bucketType) in buckets)
                {
                    try
                    {
                        await _storage.DeleteFile(bucketId, publicId);
                        
                        _logger.LogInformation("File deleted successfully from Appwrite {BucketType} bucket. FileId: {FileId}", 
                            bucketType, publicId);

                        return new FileDeletionResult
                        {
                            Success = true,
                            PublicId = publicId,
                            Result = "deleted"
                        };
                    }
                    catch (AppwriteException ex) when (ex.Code == 404)
                    {
                        // File not found in this bucket, try next bucket
                        continue;
                    }
                }

                // File not found in any bucket
                _logger.LogWarning("File not found for deletion in any bucket. FileId: {FileId}", publicId);
                return new FileDeletionResult
                {
                    Success = false,
                    PublicId = publicId,
                    ErrorMessage = "File not found"
                };
            }
            catch (AppwriteException ex)
            {
                _logger.LogError(ex, "Failed to delete file from Appwrite: {Message}", ex.Message);
                return new FileDeletionResult 
                { 
                    Success = false, 
                    PublicId = publicId, 
                    ErrorMessage = $"Failed to delete file: {ex.Message}" 
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during file deletion");
                return new FileDeletionResult 
                { 
                    Success = false, 
                    PublicId = publicId, 
                    ErrorMessage = "An unexpected error occurred during file deletion" 
                };
            }
        }

        /// <summary>
        /// Deletes a file from Appwrite using its URL by extracting the public ID
        /// </summary>
        public async Task<FileDeletionResult> DeleteFileByUrlAsync(string fileUrl, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(fileUrl))
            {
                _logger.LogWarning("File deletion by URL failed: fileUrl is null or empty");
                return new FileDeletionResult
                {
                    Success = false,
                    PublicId = string.Empty,
                    ErrorMessage = "File URL is required"
                };
            }

            // Validate URL format and project
            if (!FileUrlParser.IsValidAppwriteUrl(fileUrl))
            {
                _logger.LogWarning("File deletion by URL failed: Invalid Appwrite URL format. URL: {FileUrl}", fileUrl);
                return new FileDeletionResult
                {
                    Success = false,
                    PublicId = string.Empty,
                    ErrorMessage = "Invalid Appwrite file URL format"
                };
            }

            if (!FileUrlParser.IsFromExpectedProject(fileUrl, _config.ProjectId))
            {
                _logger.LogWarning("File deletion by URL failed: URL does not belong to current project. URL: {FileUrl}, ExpectedProject: {ProjectId}", 
                    fileUrl, _config.ProjectId);
                return new FileDeletionResult
                {
                    Success = false,
                    PublicId = string.Empty,
                    ErrorMessage = "File URL does not belong to the current project"
                };
            }

            // Extract public ID
            var publicId = ExtractPublicIdFromUrl(fileUrl);
            if (string.IsNullOrEmpty(publicId))
            {
                _logger.LogWarning("File deletion by URL failed: Could not extract public ID from URL. URL: {FileUrl}", fileUrl);
                return new FileDeletionResult
                {
                    Success = false,
                    PublicId = string.Empty,
                    ErrorMessage = "Could not extract file ID from URL"
                };
            }

            _logger.LogInformation("Extracted public ID {PublicId} from URL {FileUrl}", publicId, fileUrl);

            // Use the existing DeleteFileAsync method
            return await DeleteFileAsync(publicId, cancellationToken);
        }

        /// <summary>
        /// Extracts the public ID from an Appwrite file URL
        /// </summary>
        public string? ExtractPublicIdFromUrl(string fileUrl)
        {
            return FileUrlParser.ExtractAppwriteFileId(fileUrl);
        }

        /// <summary>
        /// Gets a page of resources from Appwrite with cursor-based pagination
        /// </summary>
        public async Task<FileResourcesPageDto> GetResourcesPageAsync(int maxResults = 100, string? nextCursor = null, string resourceType = "image", CancellationToken cancellationToken = default)
        {
            try
            {
                var bucketId = resourceType.ToLower() switch
                {
                    "image" => _config.ImagesBucketId,
                    "video" => _config.VideosBucketId,
                    "audio" => _config.VideosBucketId, // Audio files stored in video bucket
                    "document" => _config.DocumentsBucketId,
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

        private FileUploadResult MapToFileUploadResult(Appwrite.Models.File appwriteFile, string resourceType)
        {
            var viewUrl = GetFileViewUrl(appwriteFile.BucketId, appwriteFile.Id);
            
            return new FileUploadResult
            {
                Success = true,
                PublicId = appwriteFile.Id,
                Url = viewUrl,
                SecureUrl = viewUrl,
                Bytes = appwriteFile.SizeOriginal,
                Format = Path.GetExtension(appwriteFile.Name).TrimStart('.'),
                ResourceType = resourceType,
                CreatedAt = DateTime.Parse(appwriteFile.CreatedAt),
                Metadata = new Dictionary<string, object>
                {
                    ["name"] = appwriteFile.Name,
                    ["mimeType"] = appwriteFile.MimeType,
                    ["bucketId"] = appwriteFile.BucketId,
                    ["updatedAt"] = appwriteFile.UpdatedAt
                }
            };
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

        private void ValidateImageFile(IFormFile file)
        {
            ValidateFile(file, FileType.Image, "image");
        }

        private void ValidateVideoFile(IFormFile file)
        {
            ValidateFile(file, FileType.Video, "video/audio");
        }

        private void ValidateDocumentFile(IFormFile file)
        {
            ValidateFile(file, FileType.Document, "document");
        }

        private void ValidateFile(IFormFile file, FileType expectedType, string typeName)
        {
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("{TypeName} validation failed: File is null or empty", typeName);
                throw ApiException.BadRequest("INVALID_FILE", "File is required and cannot be empty");
            }

            _logger.LogInformation("Validating {TypeName} file: {FileName}, Size: {FileSize}MB ({FileSizeBytes} bytes), ContentType: {ContentType}", 
                typeName, file.FileName, 
                Math.Round((double)file.Length / (1024 * 1024), 2), 
                file.Length, 
                file.ContentType);

            if (!_mimeTypeMapping.ContainsKey(file.ContentType) || _mimeTypeMapping[file.ContentType] != expectedType)
            {
                _logger.LogWarning("{TypeName} validation failed: Invalid content type {ContentType} for file {FileName}", 
                    typeName, file.ContentType, file.FileName);
                throw ApiException.BadRequest("INVALID_FILE_TYPE", $"File type {file.ContentType} is not supported for {typeName}");
            }

            var maxSizeBytes = _config.MaxFileSizeMB * 1024 * 1024;
            _logger.LogInformation("{TypeName} file size check: File={FileSize}MB, MaxAllowed={MaxSize}MB", 
                typeName, Math.Round((double)file.Length / (1024 * 1024), 2), _config.MaxFileSizeMB);
                
            if (file.Length > maxSizeBytes)
            {
                _logger.LogWarning("{TypeName} validation failed: File size {FileSize}MB exceeds maximum {MaxSize}MB", 
                    typeName, Math.Round((double)file.Length / (1024 * 1024), 2), _config.MaxFileSizeMB);
                throw ApiException.BadRequest("FILE_TOO_LARGE", $"File size exceeds maximum allowed size of {_config.MaxFileSizeMB}MB");
            }

            _logger.LogInformation("{TypeName} file validation successful: {FileName}", typeName, file.FileName);
        }

        #endregion
    }
}
