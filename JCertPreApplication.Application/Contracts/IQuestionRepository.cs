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
        /// Gets all questions with their related attachments.
        /// </summary>
        /// <returns>List of questions including attachments.</returns>
        Task<List<Question>> GetQuestionsWithAttachmentsAsync();

        /// <summary>
        /// Gets all questions with their related choices and attachments.
        /// </summary>
        /// <returns>List of questions including choices and attachments.</returns>
        Task<List<Question>> GetQuestionsWithDetailsAsync();
    }
}
