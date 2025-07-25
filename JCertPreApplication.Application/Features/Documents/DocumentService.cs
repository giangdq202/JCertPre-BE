using AutoMapper;
using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.Document;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace JCertPreApplication.Application.Features.Documents
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly ILessonRepository _lessonRepository;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IMapper _mapper;
        
        // Phân loại file types
        private readonly string[] _allowedImageTypes = { 
            "image/jpeg", "image/jpg", "image/png", "image/gif", 
            "image/bmp", "image/webp", "image/svg+xml"
        };
        
        private readonly string[] _allowedVideoTypes = { 
            "video/mp4", "video/avi", "video/mov", "video/wmv", 
            "video/flv", "video/webm", "video/mkv", "video/3gp"
        };
        
        private readonly string[] _allowedDocumentTypes = {
            "application/pdf",
            "application/msword",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            "application/vnd.ms-powerpoint",
            "application/vnd.openxmlformats-officedocument.presentationml.presentation",
            "text/plain",
            "application/rtf"
        };

        public DocumentService(
            IDocumentRepository documentRepository,
            ILessonRepository lessonRepository,
            ICloudinaryService cloudinaryService,
            IMapper mapper)
        {
            _documentRepository = documentRepository;
            _lessonRepository = lessonRepository;
            _cloudinaryService = cloudinaryService;
            _mapper = mapper;
        }

        public async Task<DocumentDto> UploadDocumentAsync(CreateDocumentDto createDocumentDto)
        {
            // Kiểm tra tính hợp lệ của lessonId
            var lesson = await _lessonRepository.GetByIdAsync(createDocumentDto.lessonId);
            if (lesson == null)
            {
                throw ApiException.NotFound("Lesson", createDocumentDto.lessonId);
            }

            var file = createDocumentDto.file;
            // Kiểm tra file
            if (file == null || file.Length == 0)
            {
                throw ApiException.BadRequest("INVALID_FILE", "File is required");
            }

            // Kiểm tra loại file dựa trên content type
            var fileType = GetFileType(file);
            if (fileType == FileType.Unsupported)
            {
                throw ApiException.BadRequest("INVALID_FILE_TYPE", "Unsupported file type");
            }

            // Kiểm tra kích thước file (ví dụ: tối đa 100MB)
            const long maxFileSize = 100 * 1024 * 1024; // 100MB
            if (file.Length > maxFileSize)
            {
                throw ApiException.BadRequest("FILE_TOO_LARGE", "File size exceeds maximum limit");
            }

            try
            {
                // Upload file theo loại tương ứng và lấy publicId
                var (fileUrl, publicId) = await UploadFileByType(file, fileType);

                // Tạo Document entity - lưu publicId vào documentName
                var document = new Document
                {
                    documentId = Guid.NewGuid(),
                    lessonId = createDocumentDto.lessonId,
                    documentName = publicId, // Lưu publicId thay vì tên file gốc
                    fileUrl = fileUrl,
                    uploadedAt = DateTime.UtcNow
                };

                // Lưu vào database
                await _documentRepository.InsertAsync(document);
                await _documentRepository.SaveChangesAsync();

                return _mapper.Map<DocumentDto>(document);
            }
            catch (Exception ex) when (!(ex is ApiException))
            {
                throw ApiException.InternalServerError("UPLOAD_FAILED", "Failed to upload document");
            }
        }

        public async Task<DocumentDto?> GetDocumentByIdAsync(Guid documentId)
        {
            var document = await _documentRepository.GetByIdAsync(documentId);
            if (document == null)
            {
                throw ApiException.NotFound("Document", documentId);
            }

            return _mapper.Map<DocumentDto>(document);
        }

        public async Task<ICollection<DocumentDto>> GetDocumentsByLessonIdAsync(Guid lessonId)
        {
            var documents = await _documentRepository.GetDocumentsByLessonIdAsync(lessonId);
            return _mapper.Map<ICollection<DocumentDto>>(documents);
        }

        public async Task<bool> DeleteDocumentAsync(Guid documentId)
        {
            var document = await _documentRepository.GetByIdAsync(documentId);
            if (document == null)
            {
                throw ApiException.NotFound("Document", documentId);
            }

            try
            {
                // Sử dụng documentName (chính là publicId) để xóa file
                if (!string.IsNullOrEmpty(document.documentName))
                {
                    await DeleteFileByPublicId(document.documentName, document.fileUrl);
                }

                // Xóa document khỏi database
                await _documentRepository.DeleteAsync(document);
                await _documentRepository.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                throw ApiException.InternalServerError("DELETE_FAILED", "Failed to delete document");
            }
        }

        #region Private Helper Methods

        private enum FileType
        {
            Image,
            Video,
            Document,
            Unsupported
        }

        private FileType GetFileType(IFormFile file)
        {
            var contentType = file.ContentType?.ToLowerInvariant();
            
            if (string.IsNullOrEmpty(contentType))
            {
                return FileType.Unsupported;
            }

            if (_allowedImageTypes.Contains(contentType))
            {
                return FileType.Image;
            }

            if (_allowedVideoTypes.Contains(contentType))
            {
                return FileType.Video;
            }

            if (_allowedDocumentTypes.Contains(contentType))
            {
                return FileType.Document;
            }

            return FileType.Unsupported;
        }

        private async Task<(string fileUrl, string publicId)> UploadFileByType(IFormFile file, FileType fileType)
        {
            return fileType switch
            {
                FileType.Image => await UploadImageFile(file),
                FileType.Video => await UploadVideoFile(file),
                FileType.Document => await UploadDocumentFile(file),
                _ => throw ApiException.BadRequest("UNSUPPORTED_FILE_TYPE", "File type is not supported")
            };
        }

        private async Task<(string fileUrl, string publicId)> UploadImageFile(IFormFile file)
        {
            var uploadResult = await _cloudinaryService.UploadImageAsync(file);
            
            if (uploadResult == null || string.IsNullOrEmpty(uploadResult.SecureUrl?.ToString()) || string.IsNullOrEmpty(uploadResult.PublicId))
            {
                throw ApiException.InternalServerError("UPLOAD_FAILED", "Failed to upload image to cloud storage");
            }

            return (uploadResult.SecureUrl.ToString(), uploadResult.PublicId);
        }

        private async Task<(string fileUrl, string publicId)> UploadVideoFile(IFormFile file)
        {
            var uploadResult = await _cloudinaryService.UploadVideoAsync(file);
            
            if (uploadResult == null || string.IsNullOrEmpty(uploadResult.SecureUrl?.ToString()) || string.IsNullOrEmpty(uploadResult.PublicId))
            {
                throw ApiException.InternalServerError("UPLOAD_FAILED", "Failed to upload video to cloud storage");
            }

            return (uploadResult.SecureUrl.ToString(), uploadResult.PublicId);
        }

        private async Task<(string fileUrl, string publicId)> UploadDocumentFile(IFormFile file)
        {
            var uploadResult = await _cloudinaryService.UploadRawFileAsync(file);
            
            if (uploadResult == null || string.IsNullOrEmpty(uploadResult.SecureUrl?.ToString()) || string.IsNullOrEmpty(uploadResult.PublicId))
            {
                throw ApiException.InternalServerError("UPLOAD_FAILED", "Failed to upload document to cloud storage");
            }

            return (uploadResult.SecureUrl.ToString(), uploadResult.PublicId);
        }

        private async Task DeleteFileByPublicId(string publicId, string fileUrl)
        {
            var fileType = GetFileTypeFromUrl(fileUrl);
            
            switch (fileType)
            {
                case FileType.Image:
                    await _cloudinaryService.DeleteImageAsync(publicId);
                    break;
                case FileType.Video:
                    await _cloudinaryService.DeleteVideoAsync(publicId);
                    break;
                case FileType.Document:
                    await _cloudinaryService.DeleteRawFileAsync(publicId);
                    break;
                default:
                    // Fallback to raw file deletion
                    await _cloudinaryService.DeleteRawFileAsync(publicId);
                    break;
            }
        }

        private FileType GetFileTypeFromUrl(string url)
        {
            try
            {
                var uri = new Uri(url);
                var path = uri.AbsolutePath.ToLowerInvariant();
                
                if (path.Contains("/image/"))
                {
                    return FileType.Image;
                }
                else if (path.Contains("/video/"))
                {
                    return FileType.Video;
                }
                else if (path.Contains("/raw/"))
                {
                    return FileType.Document;
                }
                
                return FileType.Document; // Default fallback
            }
            catch
            {
                return FileType.Document; // Default fallback
            }
        }

        #endregion
    }
} 