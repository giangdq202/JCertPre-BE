using System.Linq.Expressions;
using JCertPreApplication.Domain.Entities;

namespace JCertPreApplication.Application.Contracts
{
    /// <summary>
    /// Repository interface for Question entity.
    /// Inherits generic CRUD operations and allows for custom Question-specific queries.
    /// </summary>
    public interface IQuestionRepository : IGenericRepository<Question>
    {
        /// <summary>
        /// Gets all questions with their related choices and attachments.
        /// </summary>
        /// <returns>List of questions including choices and attachments.</returns>
        Task<IEnumerable<Question>> GetQuestionsWithDetailsAsync();

        /// <summary>
        /// Retrieves a specified number of random question IDs for a given sub-content.
        /// </summary>
        /// <param name="subContentId">The ID of the sub-content to retrieve questions for.</param>
        /// <param name="questionCount">The number of random questions to retrieve.</param>
        /// <param name="pointPerQuestion">The point value for each question.</param>
        /// <returns>A list of GUIDs representing the IDs of the random questions.</returns>
        Task<List<Guid>> GetRandomQuestionIdsAsync(Guid subContentId, int questionCount, int pointPerQuestion);

        /// <summary>
        /// Efficiently count questions matching a predicate.
        /// </summary>
        /// <param name="predicate">The condition to be satisfied.</param>
        /// <returns>The number of questions that satisfy the condition.</returns>
        Task<int> CountAsync(Expression<Func<Question, bool>> predicate);

        /// <summary>
        /// Gets a list of questions by their IDs.
        /// </summary>
        /// <param name="questionIds">List of question IDs.</param>
        /// <returns>List of Question entities.</returns>
        Task<List<Question>> GetByIdsAsync(IEnumerable<Guid> questionIds);
    }
}
