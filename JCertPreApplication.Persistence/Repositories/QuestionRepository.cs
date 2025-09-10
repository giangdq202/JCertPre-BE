using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace JCertPreApplication.Persistence.Repositories
{
    /// <summary>
    /// Concrete repository for Question entity.
    /// Inherits generic CRUD operations and implements custom queries.
    /// </summary>
    public class QuestionRepository : GenericRepository<Question>, IQuestionRepository
    {
        public QuestionRepository(JCertPreDatabaseContext context) : base(context) { }

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

        /// <summary>
        /// Get random question IDs for a subcontent and point, excluding already used question IDs.
        /// If not enough, try with point less than 1, then greater than 1.
        /// </summary>
        public async Task<List<Guid>> GetRandomQuestionIdsAsync(
            Guid subContentId,
            int questionCount,
            int pointPerQuestion)
        {
            var result = new List<Guid>();

            // 1. Try to get questions with exact point
            var exactIds = await _dbSet
                .Where(q => q.SubContentId == subContentId
                            && q.isActive
                            && q.points == pointPerQuestion)
                .OrderBy(q => EF.Functions.Random())
                .Select(q => q.questionId)
                .Distinct()
                .Take(questionCount)
                .ToListAsync();

            result.AddRange(exactIds);

            if (result.Count >= questionCount)
                return result;

            // 2. Try with point less than pointPerQuestion (but > 0)
            int needed = questionCount - result.Count;
            var lessIds = await _dbSet
                .Where(q => q.SubContentId == subContentId
                            && q.isActive
                            && q.points < pointPerQuestion
                            && q.points > 0
                            && !result.Contains(q.questionId))
                .OrderBy(q => EF.Functions.Random())
                .Select(q => q.questionId)
                .Distinct()
                .Take(needed)
                .ToListAsync();

            result.AddRange(lessIds);

            if (result.Count >= questionCount)
                return result;

            // 3. Try with point greater than pointPerQuestion
            needed = questionCount - result.Count;
            var moreIds = await _dbSet
                .Where(q => q.SubContentId == subContentId
                            && q.isActive
                            && q.points > pointPerQuestion
                            && !result.Contains(q.questionId))
                .OrderBy(q => EF.Functions.Random())
                .Select(q => q.questionId)
                .Distinct()
                .Take(needed)
                .ToListAsync();

            result.AddRange(moreIds);

            // Always return unique IDs, at most questionCount
            return result.Distinct().Take(questionCount).ToList();
        }

        public async Task<int> CountAsync(Expression<Func<Question, bool>> predicate)
        {
            return await _dbSet.CountAsync(predicate);
        }

        public async Task<List<Question>> GetByIdsAsync(IEnumerable<Guid> questionIds)
        {
            return await _context.Questions
                .Where(q => questionIds.Contains(q.questionId)).Include(q => q.SubContent)
                .ToListAsync();
        }
    }
}
