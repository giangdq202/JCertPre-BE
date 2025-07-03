using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.Choice;
using JCertPreApplication.Application.Dtos.Question;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Entities;
using System.Linq.Expressions;

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
        public async Task<IEnumerable<QuestionDto>> GetAllAsync()
        {
            try
            {
                var questions = await _questionRepository.GetAllAsync();
                return questions.Select(MapToQuestionDto);
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
        public async Task<QuestionDto> GetByIdAsync(Guid id)
        {
            try
            {
                var question = await _questionRepository.GetByIdAsync(id);
                if (question == null)
                    throw ApiException.NotFound("Question", id);

                return MapToQuestionDto(question);
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
        public async Task<QuestionDto> CreateAsync(CreateQuestionDto createDto)
        {
            try
            {
                var question = new Question
                {
                    questionId = Guid.NewGuid(),
                    questionText = createDto.Content,
                    explanation = createDto.Explanation ?? string.Empty,
                    questionType = "multiple-choice", // Default type
                    tagId = Guid.Empty // This should be handled properly in a real implementation
                };

                var created = await _questionRepository.InsertAsync(question);
                await _questionRepository.SaveChangesAsync();

                return MapToQuestionDto(created);
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
        public async Task<QuestionDto> UpdateAsync(Guid id, UpdateQuestionDto updateDto)
        {
            try
            {
                var question = await _questionRepository.GetByIdAsync(id);
                if (question == null)
                    throw ApiException.NotFound("Question", id);

                // Update only provided fields
                if (updateDto.Content != null)
                    question.questionText = updateDto.Content;
                if (updateDto.Explanation != null)
                    question.explanation = updateDto.Explanation;

                await _questionRepository.UpdateAsync(question);
                await _questionRepository.SaveChangesAsync();

                return MapToQuestionDto(question);
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
                var question = await _questionRepository.GetByIdAsync(id);
                if (question == null)
                    throw ApiException.NotFound("Question", id);

                // Check if question has any attempts or is used in tests
                if (question.Tests.Any() || question.AttemptAnswers.Any())
                    throw ApiException.BadRequest("QUESTION_IN_USE", "Cannot delete question that is used in tests or has student attempts.");

                await _questionRepository.DeleteAsync(question);
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
        public async Task<IEnumerable<QuestionDto>> GetQuestionsWithDetailsAsync()
        {
            try
            {
                var questions = await _questionRepository.GetQuestionsWithDetailsAsync();
                return questions.Select(MapToQuestionDto);
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
        public async Task<Pagination<QuestionDto>> GetPaginatedAsync(
            string? searchTerm = null,
            bool includeChoices = false,
            int pageIndex = 1,
            int pageSize = 10)
        {
            try
            {
                // Build the predicate based on search term
                Expression<Func<Question, bool>>? predicate = null;
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    var term = searchTerm.Trim().ToLower();
                    predicate = q => q.questionText.ToLower().Contains(term) ||
                                   q.explanation.ToLower().Contains(term);
                }

                // Determine which properties to include
                string? includeProperties = includeChoices ? "Choices" : null;

                var paginatedQuestions = await _questionRepository.GetPaginationAsync(
                    predicate,
                    includeProperties,
                    pageIndex,
                    pageSize);

                return new Pagination<QuestionDto>
                {
                    Items = paginatedQuestions.Items.Select(MapToQuestionDto).ToList(),
                    TotalItemsCount = paginatedQuestions.TotalItemsCount,
                    PageIndex = paginatedQuestions.PageIndex,
                    PageSize = paginatedQuestions.PageSize
                };
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

        private static QuestionDto MapToQuestionDto(Question question)
        {
            return new QuestionDto
            {
                Id = question.questionId,
                Content = question.questionText,
                Explanation = question.explanation,
                Points = 1, // This should be added to the Question entity if needed
                CreatedAt = DateTime.UtcNow, // These should be added to the Question entity
                UpdatedAt = null,
                Choices = question.Choices?.Select(c => new ChoiceReadDto
                {
                    Id = c.choiceId,
                    Content = c.choiceText,
                    IsCorrect = c.isCorrect,
                    QuestionId = c.questionId
                }).ToList()
            };
        }
    }
}