using JCertPreApplication.Application.Dtos.Document;
using Microsoft.AspNetCore.Http;

namespace JCertPreApplication.Application.Features.Documents
{
    public interface IDocumentService
    {
        Task<DocumentDto> UploadImageDocumentAsync(CreateDocumentDto createDocumentDto);
        Task<DocumentDto> UploadVideoDocumentAsync(CreateDocumentDto createDocumentDto);
        Task<DocumentDto> UploadRawDocumentAsync(CreateDocumentDto createDocumentDto);
        Task<DocumentDto?> GetDocumentByIdAsync(Guid documentId);
        Task<ICollection<DocumentDto>> GetDocumentsByLessonIdAsync(Guid lessonId);
        Task<bool> DeleteDocumentAsync(Guid documentId);
    }
}
