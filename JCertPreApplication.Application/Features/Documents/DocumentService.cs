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
        private readonly string[] _allowedFileExtensions = { 
            // Documents
            ".pdf", ".doc", ".docx", ".ppt", ".pptx", 
            // Videos
            ".mp4", ".avi", ".mov", 
            // Images
            ".jpg", ".jpeg", ".png", ".gif" 
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

            // Kiểm tra loại file (có thể mở rộng thêm)
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedFileExtensions.Contains(fileExtension))
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
                // Tải file lên Cloudinary
                var uploadResult = await _cloudinaryService.UploadRawFileAsync(file);
                if (uploadResult == null || string.IsNullOrEmpty(uploadResult.SecureUrl?.ToString()))
                {
                    throw ApiException.InternalServerError("UPLOAD_FAILED", "Failed to upload file to cloud storage");
                }

                // Tạo Document entity
                var document = new Document
                {
                    documentId = Guid.NewGuid(),
                    lessonId = createDocumentDto.lessonId,
                    documentName = Path.GetFileNameWithoutExtension(file.FileName),
                    fileUrl = uploadResult.SecureUrl.ToString(),
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

        public async Task<DocumentDto?> UpdateDocumentAsync(Guid documentId, UpdateDocumentDto updateRequest)
        {
            var document = await _documentRepository.GetByIdAsync(documentId);
            if (document == null)
            {
                throw ApiException.NotFound("Document", documentId);
            }

            try
            {
                // Cập nhật tên document nếu được cung cấp
                if (!string.IsNullOrWhiteSpace(updateRequest.documentName))
                {
                    document.documentName = updateRequest.documentName;
                }

                // Nếu có file mới được cung cấp
                if (updateRequest.newFile != null && updateRequest.newFile.Length > 0)
                {
                    // Kiểm tra loại file
                    var fileExtension = Path.GetExtension(updateRequest.newFile.FileName).ToLowerInvariant();
                    if (!_allowedFileExtensions.Contains(fileExtension))
                    {
                        throw ApiException.BadRequest("INVALID_FILE_TYPE", "Unsupported file type");
                    }

                    // Lấy public ID từ URL cũ
                    var publicId = GetPublicIdFromUrl(document.fileUrl);
                    if (!string.IsNullOrEmpty(publicId))
                    {
                        // Xóa file cũ trên Cloudinary
                        await _cloudinaryService.DeleteRawFileAsync(publicId);
                    }

                    // Tải file mới lên Cloudinary
                    var uploadResult = await _cloudinaryService.UploadRawFileAsync(updateRequest.newFile);
                    if (uploadResult == null || string.IsNullOrEmpty(uploadResult.SecureUrl?.ToString()))
                    {
                        throw ApiException.InternalServerError("UPLOAD_FAILED", "Failed to upload new file to cloud storage");
                    }

                    document.fileUrl = uploadResult.SecureUrl.ToString();
                    
                    // Cập nhật tên file nếu không có tên mới được cung cấp
                    if (string.IsNullOrWhiteSpace(updateRequest.documentName))
                    {
                        document.documentName = Path.GetFileNameWithoutExtension(updateRequest.newFile.FileName);
                    }
                }

                // Lưu thay đổi
                await _documentRepository.UpdateAsync(document);
                await _documentRepository.SaveChangesAsync();

                return _mapper.Map<DocumentDto>(document);
            }
            catch (Exception ex) when (!(ex is ApiException))
            {
                throw ApiException.InternalServerError("UPDATE_FAILED", "Failed to update document");
            }
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
                // Lấy public ID từ URL
                var publicId = GetPublicIdFromUrl(document.fileUrl);
                if (!string.IsNullOrEmpty(publicId))
                {
                    // Xóa file trên Cloudinary
                    await _cloudinaryService.DeleteRawFileAsync(publicId);
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

        private string? GetPublicIdFromUrl(string url)
        {
            try
            {
                // Giả sử URL có dạng: https://res.cloudinary.com/your-cloud-name/raw/upload/v1234567890/folder/file.pdf
                var uri = new Uri(url);
                var segments = uri.Segments;
                var uploadIndex = Array.IndexOf(segments, "upload/");
                if (uploadIndex >= 0 && uploadIndex < segments.Length - 1)
                {
                    // Lấy phần sau "upload/" và bỏ phần mở rộng
                    var fullPath = string.Join("", segments.Skip(uploadIndex + 1));
                    return Path.GetFileNameWithoutExtension(fullPath);
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
    }
} 