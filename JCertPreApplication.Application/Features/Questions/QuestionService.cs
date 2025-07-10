using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.Question;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;
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
        private readonly ISubContentRepository _subContentRepository;

        public QuestionService(IQuestionRepository questionRepository, ISubContentRepository subContentRepository)
        {
            _questionRepository = questionRepository ?? throw new ArgumentNullException(nameof(questionRepository));
            _subContentRepository = subContentRepository ?? throw new ArgumentNullException(nameof(subContentRepository));
        }

        /// <summary>
        /// Retrieves all questions.
        /// </summary>
        public async Task<IEnumerable<Question>> GetAllAsync()
        {
            try
            {
                var questions = await _questionRepository.GetAllAsync("SubContent");
                return questions;
            }
            catch (ApiException) { throw; }
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
            catch (ApiException) { throw; }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("QUESTION_SERVICE_ERROR", $"An error occurred while retrieving the question: {ex.Message}");
            }
        }

        /// <summary>
        /// Creates a new question.
        /// </summary>
        public async Task<Question> CreateAsync(CreateQuestionDto createDto, ContentName contentName, CourseLevel level, SubContentName subContentName)
        {
            try
            {
                // Find subcontent by enums
                var subContent = await _subContentRepository.GetFirstOrDefaultAsync(
                    s => s.ContentName == contentName && s.Level == level && s.SubContentName == subContentName
                );
                if (subContent == null)
                    throw ApiException.BadRequest("SUBCONTENT_NOT_FOUND", "SubContent does not exist for the provided ContentName, Level, and SubContentName.");

                var question = new Question
                {
                    questionId = Guid.NewGuid(),
                    questionText = createDto.Content,
                    explanation = createDto.Explanation ?? string.Empty,
                    questionType = "multiple-choice",
                    points = createDto.Points,
                    SubContentId = subContent.SubContentId
                };

                var created = await _questionRepository.InsertAsync(question);
                await _questionRepository.SaveChangesAsync();

                created = await _questionRepository.GetFirstOrDefaultAsync(q => q.questionId == created.questionId, "SubContent");

                return created;
            }
            catch (ApiException) { throw; }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("QUESTION_SERVICE_ERROR", $"An error occurred while creating the question: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates an existing question.
        /// </summary>
        public async Task<Question> UpdateAsync(Guid id, UpdateQuestionDto updateDto)
        {
            try
            {
                var question = await _questionRepository.GetByIdAsync(id);
                if (question == null)
                    throw ApiException.NotFound("Question", id);

                if (updateDto.Content != null)
                    question.questionText = updateDto.Content;
                if (updateDto.Explanation != null)
                    question.explanation = updateDto.Explanation;
                if (updateDto.Points.HasValue)
                    question.points = updateDto.Points.Value;

                await _questionRepository.UpdateAsync(question);
                await _questionRepository.SaveChangesAsync();

                return question;
            }
            catch (ApiException) { throw; }
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

                if (question.TestQuestions.Count > 0 || question.AttemptAnswers.Count > 0)
                    throw ApiException.BadRequest("QUESTION_IN_USE", "Cannot delete question that is used in tests or has student attempts.");

                await _questionRepository.DeleteAsync(question);
                await _questionRepository.SaveChangesAsync();
            }
            catch (ApiException) { throw; }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("QUESTION_SERVICE_ERROR", $"An error occurred while deleting the question: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves paginated questions with details (choices, attachments), not including tag.
        /// </summary>
        public async Task<Pagination<Question>> GetPaginatedWithDetailsAsync(
            string? searchTerm,
            int pageIndex,
            int pageSize,
            ContentName? contentName = null,
            CourseLevel? level = null,
            SubContentName? subContentName = null)
        {
            try
            {
                Expression<Func<Question, bool>>? predicate = null;

                if (!string.IsNullOrWhiteSpace(searchTerm) || contentName.HasValue || level.HasValue || subContentName.HasValue)
                {
                    predicate = q =>
                        (string.IsNullOrEmpty(searchTerm) ||
                            q.questionText.ToLower().Contains(searchTerm.ToLower()) ||
                            q.explanation.ToLower().Contains(searchTerm.ToLower()))
                        && (!contentName.HasValue || q.SubContent.ContentName == contentName.Value)
                        && (!level.HasValue || q.SubContent.Level == level.Value)
                        && (!subContentName.HasValue || q.SubContent.SubContentName == subContentName.Value);
                }

                string includeProperties = "Choices,QuestionAttachments,SubContent";

                var paginatedQuestions = await _questionRepository.GetPaginationAsync(
                    predicate,
                    includeProperties,
                    pageIndex,
                    pageSize);

                return new Pagination<Question>
                {
                    Items = paginatedQuestions.Items,
                    TotalItemsCount = paginatedQuestions.TotalItemsCount,
                    PageIndex = paginatedQuestions.PageIndex,
                    PageSize = paginatedQuestions.PageSize
                };
            }
            catch (ApiException) { throw; }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("QUESTION_SERVICE_ERROR", $"An error occurred while retrieving paginated questions: {ex.Message}");
            }
        }
    }
}