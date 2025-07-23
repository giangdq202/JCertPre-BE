using JCertPreApplication.Application.Dtos.Document;
using Microsoft.AspNetCore.Http;

namespace JCertPreApplication.Application.Features.Documents
{
    public interface IDocumentService
    {
        Task<DocumentDto> UploadDocumentAsync(CreateDocumentDto createDocumentDto);
        Task<DocumentDto> GetDocumentByIdAsync(Guid documentId);
        Task<ICollection<DocumentDto>> GetDocumentsByLessonIdAsync(Guid lessonId);
        Task<DocumentDto> UpdateDocumentAsync(Guid documentId, UpdateDocumentDto updateRequest);
        Task<bool> DeleteDocumentAsync(Guid documentId);
    }
} 