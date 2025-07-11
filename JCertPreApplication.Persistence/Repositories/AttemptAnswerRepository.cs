using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Persistence.DatabaseContext;

namespace JCertPreApplication.Persistence.Repositories
{
    public class AttemptAnswerRepository : GenericRepository<AttemptAnswer>, IAttemptAnswerRepository
    {
        public AttemptAnswerRepository(JCertPreDatabaseContext context) : base(context) { }
        // Implement custom methods if needed
    }
}