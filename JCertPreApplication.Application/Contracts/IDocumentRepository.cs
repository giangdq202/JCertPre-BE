using JCertPreApplication.Domain.Entities;

namespace JCertPreApplication.Application.Contracts
{
    public interface IDocumentRepository : IGenericRepository<Document>
    {
        Task<ICollection<Document>> GetDocumentsByLessonIdAsync(Guid lessonId);
    }
} 