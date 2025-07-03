using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;

namespace JCertPreApplication.Application.Features.Questions
{
    /// <summary>
    /// Service for handling business logic related to Question entities.
    /// Implements exception handling and follows Clean Architecture best practices.
    /// </summary>
    public class QuestionService : IQuestionService
    {
        private readonly IQuestionRepository _questionRepository;

        public QuestionService(IQuestionRepository questionRepository)
        {
            _questionRepository = questionRepository ?? throw new ArgumentNullException(nameof(questionRepository));
        }

        /// <summary>
        /// Retrieves all questions.
        /// </summary>
        public async Task<IEnumerable<Question>> GetAllAsync()
        {
            try
            {
                return await _questionRepository.GetAllAsync();
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("QUESTION_SERVICE_ERROR", $"An error occurred while retrieving questions: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves a question by its ID.
        /// Throws ApiException.NotFound if question doesn't exist.
        /// </summary>
        public async Task<Question> GetByIdAsync(Guid id)
        {
            try
            {
                var question = await _questionRepository.GetByIdAsync(id);
                if (question == null)
                    throw ApiException.NotFound("Question", id);

                return question;
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("QUESTION_SERVICE_ERROR", $"An error occurred while retrieving the question: {ex.Message}");
            }
        }

        /// <summary>
        /// Creates a new question.
        /// </summary>
        public async Task<Question> CreateAsync(Question question)
        {
            try
            {
                var created = await _questionRepository.InsertAsync(question);
                await _questionRepository.SaveChangesAsync();
                return created;
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("QUESTION_SERVICE_ERROR", $"An error occurred while creating the question: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates an existing question.
        /// </summary>
        public async Task UpdateAsync(Question question)
        {
            try
            {
                // Check if question exists before updating
                var existingQuestion = await _questionRepository.GetByIdAsync(question.questionId);
                if (existingQuestion == null)
                    throw ApiException.NotFound("Question", question.questionId);

                await _questionRepository.UpdateAsync(question);
                await _questionRepository.SaveChangesAsync();
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("QUESTION_SERVICE_ERROR", $"An error occurred while updating the question: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes a question by its ID.
        /// </summary>
        public async Task DeleteAsync(Guid id)
        {
            try
            {
                var entity = await _questionRepository.GetByIdAsync(id);
                if (entity == null)
                    throw ApiException.NotFound("Question", id);

                await _questionRepository.DeleteAsync(entity);
                await _questionRepository.SaveChangesAsync();
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("QUESTION_SERVICE_ERROR", $"An error occurred while deleting the question: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves all questions with their related choices and attachments.
        /// </summary>
        public async Task<IEnumerable<Question>> GetQuestionsWithDetailsAsync()
        {
            try
            {
                return await _questionRepository.GetQuestionsWithDetailsAsync();
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("QUESTION_SERVICE_ERROR", $"An error occurred while retrieving question details: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves paginated questions with optional filtering and includes.
        /// </summary>
        public async Task<Pagination<Question>> GetPagingAsync(
            Expression<Func<Question, bool>>? predicate = null,
            string? includeProperties = null,
            int pageIndex = 1,
            int pageSize = 10)
        {
            try
            {
                return await _questionRepository.GetPaginationAsync(predicate, includeProperties, pageIndex, pageSize);
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("QUESTION_SERVICE_ERROR", $"An error occurred while retrieving paginated questions: {ex.Message}");
            }
        }
    }
}