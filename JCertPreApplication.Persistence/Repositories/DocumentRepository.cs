using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace JCertPreApplication.Persistence.Repositories
{
    public class DocumentRepository : GenericRepository<Document>, IDocumentRepository
    {
        public DocumentRepository(JCertPreDatabaseContext context) : base(context)
        {
        }

        public async Task<ICollection<Document>> GetDocumentsByLessonIdAsync(Guid lessonId)
        {
            return await _context.Documents
                .Where(d => d.lessonId == lessonId)
                .OrderBy(d => d.uploadedAt)
                .ToListAsync();
        }
    }
} 