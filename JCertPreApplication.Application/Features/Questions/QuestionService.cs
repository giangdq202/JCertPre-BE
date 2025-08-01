using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.Question;
using JCertPreApplication.Application.Dtos.QuestionAttachment;
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
        /// Retrieves a question by its ID.
        /// Throws ApiException.NotFound if question doesn't exist.
        /// </summary>
        public async Task<QuestionDto> GetByIdAsync(Guid id)
        {
            try
            {
                // Load Choices, QuestionAttachments, SubContent
                var question = await _questionRepository.GetFirstOrDefaultAsync(
                    q => q.questionId == id,
                    "Choices,QuestionAttachments,SubContent"
                );
                if (question == null)
                    throw ApiException.NotFound("Question", id);

                return MapToQuestionDto(question);
            }
            catch (ApiException) { throw; }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("QUESTION_SERVICE_ERROR", $"An error occurred while retrieving the question: {ex.Message}");
            }
        }

        /// <summary>
        /// Creates a new question. Looks up SubContent by enums from DTO.
        /// </summary>
        public async Task<QuestionDto> CreateAsync(CreateQuestionDto createDto)
        {
            try
            {
                var subContent = await _subContentRepository.GetFirstOrDefaultAsync(
                    s => s.ContentName == createDto.ContentName
                      && s.Level == createDto.Level
                      && s.SubContentName == createDto.SubContentName
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
                    difficulty = createDto.Difficulty,
                    SubContentId = subContent.SubContentId,
                    isActive = createDto.IsActive
                };

                // Insert and persist the new question
                var created = await _questionRepository.InsertAsync(question);
                await _questionRepository.SaveChangesAsync();

                // Retrieve the created question with navigation properties
                var result = await _questionRepository.GetFirstOrDefaultAsync(q => q.questionId == created.questionId, "SubContent");
                if (result == null)
                    throw ApiException.InternalServerError("QUESTION_CREATION_ERROR", "Failed to retrieve the created question.");

                return MapToQuestionDto(result);
            }
            catch (ApiException) { throw; }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("QUESTION_SERVICE_ERROR", $"An error occurred while creating the question: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates an existing question. Optionally updates SubContent if all enums are provided.
        /// Returns the updated question with SubContent navigation property loaded.
        /// </summary>
        public async Task<QuestionDto> UpdateAsync(Guid id, UpdateQuestionDto updateDto)
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
                if (updateDto.Difficulty.HasValue)
                    question.difficulty = updateDto.Difficulty.Value;
                if (updateDto.IsActive.HasValue)
                    question.isActive = updateDto.IsActive.Value;

                if (updateDto.ContentName.HasValue && updateDto.Level.HasValue && updateDto.SubContentName.HasValue)
                {
                    var subContent = await _subContentRepository.GetFirstOrDefaultAsync(
                        s => s.ContentName == updateDto.ContentName.Value
                          && s.Level == updateDto.Level.Value
                          && s.SubContentName == updateDto.SubContentName.Value
                    );
                    if (subContent == null)
                        throw ApiException.BadRequest("SUBCONTENT_NOT_FOUND", "SubContent does not exist for the provided ContentName, Level, and SubContentName.");
                    question.SubContentId = subContent.SubContentId;
                }

                await _questionRepository.UpdateAsync(question);
                await _questionRepository.SaveChangesAsync();

                var updated = await _questionRepository.GetFirstOrDefaultAsync(q => q.questionId == question.questionId, "SubContent");
                if (updated == null)
                    throw ApiException.InternalServerError("QUESTION_UPDATE_ERROR", "Failed to retrieve the updated question.");

                return MapToQuestionDto(updated);
            }
            catch (ApiException) { throw; }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("QUESTION_SERVICE_ERROR", $"An error occurred while updating the question: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes a question by its ID. Prevents deletion if the question is in use.
        /// </summary>
        public async Task DeleteAsync(Guid id)
        {
            try
            {
                // Find the question to delete
                var question = await _questionRepository.GetByIdAsync(id);
                if (question == null)
                    throw ApiException.NotFound("Question", id);

                // Prevent deletion if the question is referenced in tests or attempts
                if (question.TestQuestions.Count > 0 || question.AttemptAnswers.Count > 0)
                    throw ApiException.BadRequest("QUESTION_IN_USE", "Cannot delete question that is used in tests or has student attempts.");

                // Delete and persist
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
        /// Retrieves paginated questions with details (choices, attachments, subcontent).
        /// Supports filtering by search term, subcontent, and difficulty.
        /// </summary>
        public async Task<Pagination<QuestionDto>> GetPaginatedWithDetailsAsync(
            string? searchTerm,
            int pageIndex,
            int pageSize,
            ContentName? contentName = null,
            CourseLevel? level = null,
            SubContentName? subContentName = null,
            QuestionDifficulty? difficulty = null)
        {
            try
            {
                // Build predicate for filtering
                Expression<Func<Question, bool>>? predicate = null;

                if (!string.IsNullOrWhiteSpace(searchTerm) || contentName.HasValue || level.HasValue || subContentName.HasValue || difficulty.HasValue)
                {
                    predicate = q =>
                        (string.IsNullOrEmpty(searchTerm) ||
                            q.questionText.ToLower().Contains(searchTerm.ToLower()) ||
                            q.explanation.ToLower().Contains(searchTerm.ToLower()))
                        && (!contentName.HasValue || q.SubContent.ContentName == contentName.Value)
                        && (!level.HasValue || q.SubContent.Level == level.Value)
                        && (!subContentName.HasValue || q.SubContent.SubContentName == subContentName.Value)
                        && (!difficulty.HasValue || q.difficulty == difficulty.Value);
                }

                // Include related entities for rich API responses
                string includeProperties = "Choices,QuestionAttachments,SubContent";

                // Fetch paginated questions
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
            catch (ApiException) { throw; }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("QUESTION_SERVICE_ERROR", $"An error occurred while retrieving paginated questions: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves a question by its ID for the test. Lighter DTO for test performance.
        /// </summary>
        public async Task<QuestionForTestDto?> GetByIdForTestAsync(Guid id)
        {
            var question = await _questionRepository.GetFirstOrDefaultAsync(
                q => q.questionId == id,
                "Choices,QuestionAttachments"
            );
            if (question == null)
                return null;

            return new QuestionForTestDto
            {
                Id = question.questionId,
                Content = question.questionText,
                QuestionType = question.questionType,
                Choices = question.Choices?.Select(c => new ChoiceForTestDto
                {
                    ChoiceId = c.choiceId,
                    Content = c.choiceText
                }).ToList(),
                QuestionAttachments = question.QuestionAttachments?.Select(a => new QuestionAttachmentDto
                {
                    MediaUrl = a.mediaUrl,
                    MediaType = a.mediaType
                }).ToList()
            };
        }

        private static QuestionDto MapToQuestionDto(Question question)
        {
            var subContent = question.SubContent;
            return new QuestionDto
            {
                Id = question.questionId,
                Content = question.questionText,
                Explanation = question.explanation,
                Points = question.points,
                Difficulty = question.difficulty,
                IsActive = question.isActive,
                Choices = question.Choices?.Select(c => new ChoiceReadDto
                {
                    ChoiceId = c.choiceId,
                    Content = c.choiceText,
                    IsCorrect = c.isCorrect,
                    QuestionId = c.questionId
                }).ToList(),
                QuestionAttachments = question.QuestionAttachments?.Select(a => new QuestionAttachmentDto
                {
                    MediaUrl = a.mediaUrl,
                    MediaType = a.mediaType
                }).ToList(),
                ContentName = subContent?.ContentName.ToString() ?? "",
                ContentNameDescription = subContent != null ? EnumHelper.GetEnumDescription(subContent.ContentName) : "",
                Level = subContent?.Level.ToString() ?? "",
                LevelDescription = subContent != null ? EnumHelper.GetEnumDescription(subContent.Level) : "",
                SubContentName = subContent?.SubContentName.ToString() ?? "",
                SubContentNameDescription = subContent != null ? EnumHelper.GetEnumDescription(subContent.SubContentName) : ""
            };
        }

        /// <summary>
        /// Retrieves paginated active questions with details (choices, attachments, subcontent).
        /// Supports filtering by search term, subcontent, and difficulty.
        /// </summary>
        public async Task<Pagination<QuestionDto>> GetPaginatedActiveWithDetailsAsync(
            string? searchTerm,
            int pageIndex,
            int pageSize,
            ContentName? contentName = null,
            CourseLevel? level = null,
            SubContentName? subContentName = null,
            QuestionDifficulty? difficulty = null)
        {
            try
            {
                // Build predicate for filtering, always require isActive == true
                Expression<Func<Question, bool>> predicate = q =>
                    q.isActive &&
                    (string.IsNullOrEmpty(searchTerm) ||
                        q.questionText.ToLower().Contains(searchTerm.ToLower()) ||
                        q.explanation.ToLower().Contains(searchTerm.ToLower())) &&
                    (!contentName.HasValue || q.SubContent.ContentName == contentName.Value) &&
                    (!level.HasValue || q.SubContent.Level == level.Value) &&
                    (!subContentName.HasValue || q.SubContent.SubContentName == subContentName.Value) &&
                    (!difficulty.HasValue || q.difficulty == difficulty.Value);

                string includeProperties = "Choices,QuestionAttachments,SubContent";

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
            catch (ApiException) { throw; }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("QUESTION_SERVICE_ERROR", $"An error occurred while retrieving active paginated questions: {ex.Message}");
            }
        }
    }
}