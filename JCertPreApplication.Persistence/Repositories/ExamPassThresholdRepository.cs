using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Persistence.DatabaseContext;

namespace JCertPreApplication.Persistence.Repositories
{
    public class ExamPassThresholdRepository : GenericRepository<ExamPassThreshold>, IExamPassThresholdRepository
    {
        public ExamPassThresholdRepository(JCertPreDatabaseContext context) : base(context) { }
        // Implement custom methods if needed
    }
}