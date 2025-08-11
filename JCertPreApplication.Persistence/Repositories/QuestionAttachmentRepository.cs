using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Persistence.DatabaseContext;

namespace JCertPreApplication.Persistence.Repositories
{
    public class QuestionAttachmentRepository : GenericRepository<QuestionAttachment>, IQuestionAttachmentRepository
    {
        public QuestionAttachmentRepository(JCertPreDatabaseContext context) : base(context)
        {
        }
        // Add custom methods if needed
    }
}