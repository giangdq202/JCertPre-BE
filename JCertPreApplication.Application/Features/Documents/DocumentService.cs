using AutoMapper;
using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.Document;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace JCertPreApplication.Application.Features.Documents
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly ILessonRepository _lessonRepository;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;
        
        // Image file types - only JPG, PNG, JPEG
        private readonly string[] _imageFileTypes = {
            "image/jpeg",
            "image/jpg",
            "image/png"
        };

        // Video file types - only MP4
        private readonly string[] _videoFileTypes = {
            "video/mp4"
        };

        // Document file types - PDF, Excel, Word, PowerPoint
        private readonly string[] _rawFileTypes = {
            "application/pdf",
            // Word documents
            "application/msword",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            // Excel documents
            "application/vnd.ms-excel",
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            // PowerPoint documents
            "application/vnd.ms-powerpoint",
            "application/vnd.openxmlformats-officedocument.presentationml.presentation"
        };

        public DocumentService(
            IDocumentRepository documentRepository,
            ILessonRepository lessonRepository,
            IFileService fileService,
            IMapper mapper)
        {
            _documentRepository = documentRepository;
            _lessonRepository = lessonRepository;
            _fileService = fileService;
            _mapper = mapper;
        }

        public async Task<DocumentDto> UploadImageDocumentAsync(CreateDocumentDto createDocumentDto)
        {
            // Validate lesson exists
            var lesson = await _lessonRepository.GetByIdAsync(createDocumentDto.lessonId);
            if (lesson == null)
            {
                throw ApiException.NotFound("Lesson", createDocumentDto.lessonId);
            }

            var file = createDocumentDto.file;

            // Validate file
            if (file == null || file.Length == 0)
            {
                throw ApiException.BadRequest("INVALID_FILE", "File is required");
            }

            // Validate file type for images
            if (!_imageFileTypes.Contains(file.ContentType?.ToLowerInvariant()))
            {
                throw ApiException.BadRequest("INVALID_IMAGE_TYPE", "Only image files are allowed for this endpoint");
            }

            // Validate file size (50MB max for images)
            const long maxFileSize = 50 * 1024 * 1024;
            if (file.Length > maxFileSize)
            {
                throw ApiException.BadRequest("FILE_TOO_LARGE", "Image file size exceeds maximum limit of 50MB");
            }

            try
            {
                // Upload image file
                var uploadResult = await _fileService.UploadImageAsync(file);

                if (uploadResult == null || string.IsNullOrEmpty(uploadResult.SecureUrl?.ToString()) || string.IsNullOrEmpty(uploadResult.PublicId))
                {
                    throw ApiException.InternalServerError("UPLOAD_FAILED", "Failed to upload image to cloud storage");
                }

                // Create Document entity
                var document = new Document
                {
                    documentId = Guid.NewGuid(),
                    lessonId = createDocumentDto.lessonId,
                    documentName = uploadResult.PublicId,
                    fileUrl = uploadResult.SecureUrl.ToString(),
                    uploadedAt = DateTime.UtcNow
                };

                // Save to database
                await _documentRepository.InsertAsync(document);
                await _documentRepository.SaveChangesAsync();

                return _mapper.Map<DocumentDto>(document);
            }
            catch (Exception ex) when (!(ex is ApiException))
            {
                throw ApiException.InternalServerError("UPLOAD_FAILED", "Failed to upload image document");
            }
        }

        public async Task<DocumentDto> UploadVideoDocumentAsync(CreateDocumentDto createDocumentDto)
        {
            // Validate lesson exists
            var lesson = await _lessonRepository.GetByIdAsync(createDocumentDto.lessonId);
            if (lesson == null)
            {
                throw ApiException.NotFound("Lesson", createDocumentDto.lessonId);
            }

            var file = createDocumentDto.file;

            // Validate file
            if (file == null || file.Length == 0)
            {
                throw ApiException.BadRequest("INVALID_FILE", "File is required");
            }

            // Validate file type for videos
            if (!_videoFileTypes.Contains(file.ContentType?.ToLowerInvariant()))
            {
                throw ApiException.BadRequest("INVALID_VIDEO_TYPE", "Only video files are allowed for this endpoint");
            }

            // Validate file size (500MB max for videos)
            const long maxFileSize = 500 * 1024 * 1024;
            if (file.Length > maxFileSize)
            {
                throw ApiException.BadRequest("FILE_TOO_LARGE", "Video file size exceeds maximum limit of 500MB");
            }

            try
            {
                // Upload video file
                var uploadResult = await _fileService.UploadVideoAsync(file);

                if (uploadResult == null || string.IsNullOrEmpty(uploadResult.SecureUrl?.ToString()) || string.IsNullOrEmpty(uploadResult.PublicId))
                {
                    throw ApiException.InternalServerError("UPLOAD_FAILED", "Failed to upload video to cloud storage");
                }

                // Create Document entity
                var document = new Document
                {
                    documentId = Guid.NewGuid(),
                    lessonId = createDocumentDto.lessonId,
                    documentName = uploadResult.PublicId,
                    fileUrl = uploadResult.SecureUrl.ToString(),
                    uploadedAt = DateTime.UtcNow
                };

                // Save to database
                await _documentRepository.InsertAsync(document);
                await _documentRepository.SaveChangesAsync();

                return _mapper.Map<DocumentDto>(document);
            }
            catch (Exception ex) when (!(ex is ApiException))
            {
                throw ApiException.InternalServerError("UPLOAD_FAILED", "Failed to upload video document");
            }
        }

        public async Task<DocumentDto> UploadRawDocumentAsync(CreateDocumentDto createDocumentDto)
        {
            // Validate lesson exists
            var lesson = await _lessonRepository.GetByIdAsync(createDocumentDto.lessonId);
            if (lesson == null)
            {
                throw ApiException.NotFound("Lesson", createDocumentDto.lessonId);
            }

            var file = createDocumentDto.file;

            // Validate file
            if (file == null || file.Length == 0)
            {
                throw ApiException.BadRequest("INVALID_FILE", "File is required");
            }

            // Validate file type for raw documents
            if (!_rawFileTypes.Contains(file.ContentType?.ToLowerInvariant()))
            {
                throw ApiException.BadRequest("INVALID_DOCUMENT_TYPE", "Only document files (PDF, Word, PowerPoint, etc.) are allowed for this endpoint");
            }

            // Validate file size (100MB max for documents)
            const long maxFileSize = 100 * 1024 * 1024;
            if (file.Length > maxFileSize)
            {
                throw ApiException.BadRequest("FILE_TOO_LARGE", "Document file size exceeds maximum limit of 100MB");
            }

            try
            {
                // Upload raw document file
                var uploadResult = await _fileService.UploadRawFileAsync(file);

                if (uploadResult == null || string.IsNullOrEmpty(uploadResult.SecureUrl?.ToString()) || string.IsNullOrEmpty(uploadResult.PublicId))
                {
                    throw ApiException.InternalServerError("UPLOAD_FAILED", "Failed to upload document to cloud storage");
                }

                // Create Document entity
                var document = new Document
                {
                    documentId = Guid.NewGuid(),
                    lessonId = createDocumentDto.lessonId,
                    documentName = uploadResult.PublicId,
                    fileUrl = uploadResult.SecureUrl.ToString(),
                    uploadedAt = DateTime.UtcNow
                };

                // Save to database
                await _documentRepository.InsertAsync(document);
                await _documentRepository.SaveChangesAsync();

                return _mapper.Map<DocumentDto>(document);
            }
            catch (Exception ex) when (!(ex is ApiException))
            {
                throw ApiException.InternalServerError("UPLOAD_FAILED", "Failed to upload raw document");
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
                // Delete file from cloud storage using publicId
                if (!string.IsNullOrEmpty(document.documentName))
                {
                    await _fileService.DeleteRawFileAsync(document.documentName);
                }

                // Delete document from database
                await _documentRepository.DeleteAsync(document);
                await _documentRepository.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                throw ApiException.InternalServerError("DELETE_FAILED", "Failed to delete document");
            }
        }
    }
}
