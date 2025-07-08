using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JCertPreApplication.Persistence.Repositories
{
    /// <summary>
    /// Concrete repository for Question entity.
    /// Inherits generic CRUD operations and implements custom queries.
    /// </summary>
    public class QuestionRepository : GenericRepository<Question>, IQuestionRepository
    {
        /// <summary>
        /// Constructor injecting the database context.
        /// </summary>
        /// <param name="context">Database context</param>
        public QuestionRepository(JCertPreDatabaseContext context) : base(context)
        {
        }

        /// <summary>
        /// Gets all questions with their related attachments.
        /// </summary>
        /// <returns>List of questions including attachments.</returns>
        public async Task<IEnumerable<Question>> GetQuestionsWithAttachmentsAsync()
        {
            // Eagerly load QuestionAttachments navigation property
            return await _dbSet
                .Include(q => q.QuestionAttachments)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Gets all questions with their related choices and attachments.
        /// </summary>
        /// <returns>List of questions including choices and attachments.</returns>
        public async Task<IEnumerable<Question>> GetQuestionsWithDetailsAsync()
        {
            // Eagerly load Choices and QuestionAttachments navigation properties
            return await _dbSet
                .Include(q => q.Choices)
                .Include(q => q.QuestionAttachments)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
