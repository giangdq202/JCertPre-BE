using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.Choice;
using JCertPreApplication.Application.Dtos.Question;
using JCertPreApplication.Application.Dtos.QuestionAttachment;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System.Linq.Expressions;
using System.Text.Json;

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
        private readonly IQuestionAttachmentRepository _questionAttachmentRepository;
        private readonly IChoiceRepository _choiceRepository;
        private readonly IFileService _fileService;
        private readonly IAIIntegration _aiIntegration;

        public QuestionService(
            IQuestionRepository questionRepository,
            ISubContentRepository subContentRepository,
            IQuestionAttachmentRepository questionAttachmentRepository,
            IChoiceRepository choiceRepository,
            IFileService fileService,
            IAIIntegration aiIntegration)
        {
            _questionRepository = questionRepository ?? throw new ArgumentNullException(nameof(questionRepository));
            _subContentRepository = subContentRepository ?? throw new ArgumentNullException(nameof(subContentRepository));
            _questionAttachmentRepository = questionAttachmentRepository ?? throw new ArgumentNullException(nameof(questionAttachmentRepository));
            _choiceRepository = choiceRepository ?? throw new ArgumentNullException(nameof(choiceRepository));
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _aiIntegration = aiIntegration ?? throw new ArgumentNullException(nameof(aiIntegration));
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

                var isExisted = await _questionRepository.AnyAsync(q => q.questionText == createDto.Content);
                if (isExisted)
                    throw ApiException.BadRequest("QUESTION_TEXT_EXISTS", "A question with the same text already exists.");

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

                var created = await _questionRepository.InsertAsync(question);
                await _questionRepository.SaveChangesAsync();

                // Handle audio upload for listening questions
                if (createDto.ContentName == ContentName.Listening && createDto.AudioFile != null)
                {
                    // Use questionId as filename for uniqueness
                    var customFormFile = CreateCustomFormFile(createDto.AudioFile, question.questionId.ToString());
                    var uploadResult = await _fileService.UploadVideoAsync(customFormFile);
                    if (!uploadResult.Success || string.IsNullOrEmpty(uploadResult.Url))
                        throw ApiException.InternalServerError("AUDIO_UPLOAD_FAILED", uploadResult.ErrorMessage ?? "Audio upload failed.");

                    var attachment = new QuestionAttachment
                    {
                        attachmentId = Guid.NewGuid(),
                        questionId = created.questionId,
                        mediaUrl = uploadResult.SecureUrl ?? uploadResult.Url,
                        mediaType = "audio"
                    };
                    await _questionAttachmentRepository.InsertAsync(attachment);
                    await _questionAttachmentRepository.SaveChangesAsync();
                }

                var result = await _questionRepository.GetFirstOrDefaultAsync(q => q.questionId == created.questionId, "SubContent,QuestionAttachments");
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
                var question = await _questionRepository.GetFirstOrDefaultAsync(q => q.questionId == id, "SubContent,QuestionAttachments");
                if (question == null)
                    throw ApiException.NotFound("Question", id);

                // Efficiently check for duplicate question text if updating content
                if (updateDto.Content != null)
                {
                    var isExisted = await _questionRepository.AnyAsync(q => q.questionText == updateDto.Content && q.questionId != id);
                    if (isExisted)
                        throw ApiException.BadRequest("QUESTION_TEXT_EXISTS", "A question with the same text already exists.");
                    question.questionText = updateDto.Content;
                }
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
                var newContentName = updateDto.ContentName ?? question.SubContent?.ContentName;
                // Handle audio update for listening questions
                if (newContentName == ContentName.Listening && updateDto.AudioFile != null)
                {
                    // Find existing audio attachment
                    var existingAttachment = question.QuestionAttachments.FirstOrDefault(a => a.mediaType == "audio");
                    if (existingAttachment != null)
                    {
                        // Delete old audio file from storage using URL
                        if (!string.IsNullOrWhiteSpace(existingAttachment.mediaUrl))
                        {
                            var deleteResult = await _fileService.DeleteFileByUrlAsync(existingAttachment.mediaUrl);
                            
                            if (!deleteResult.Success)
                            {
                                // Log warning but don't fail the update if old file deletion fails
                                System.Diagnostics.Debug.WriteLine($"Warning: Failed to delete old audio file: {deleteResult.ErrorMessage}");
                            }
                        }
                    }

                    // Use questionId as filename for uniqueness
                    var customFormFile = CreateCustomFormFile(updateDto.AudioFile, question.questionId.ToString());
                    var uploadResult = await _fileService.UploadVideoAsync(customFormFile);
                    if (!uploadResult.Success || string.IsNullOrEmpty(uploadResult.Url))
                        throw ApiException.InternalServerError("AUDIO_UPLOAD_FAILED", uploadResult.ErrorMessage ?? "Audio upload failed.");

                    if (existingAttachment != null)
                    {
                        // Update existing attachment
                        existingAttachment.mediaUrl = uploadResult.SecureUrl ?? uploadResult.Url;
                    }
                    else
                    {
                        // Create new attachment
                        var newAttachment = new QuestionAttachment
                        {
                            attachmentId = Guid.NewGuid(),
                            questionId = question.questionId,
                            mediaUrl = uploadResult.SecureUrl ?? uploadResult.Url,
                            mediaType = "audio"
                        };
                        await _questionAttachmentRepository.InsertAsync(newAttachment);
                    }
                    await _questionAttachmentRepository.SaveChangesAsync();
                }

                await _questionRepository.UpdateAsync(question);
                await _questionRepository.SaveChangesAsync();

                var updated = await _questionRepository.GetFirstOrDefaultAsync(q => q.questionId == question.questionId, "SubContent,QuestionAttachments");
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
            try
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
            catch (ApiException) { throw; }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("QUESTION_SERVICE_ERROR", $"An error occurred while retrieving the question for test: {ex.Message}");
            }
        }

        private static QuestionDto MapToQuestionDto(Question question)
        {
            try
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
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("QUESTION_SERVICE_ERROR", $"An error occurred while mapping question to DTO: {ex.Message}");
            }
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

        /// <summary>
        /// Imports questions from a JSON file. Supports batch import with error reporting.
        /// </summary>
        public async Task<ImportQuestionsResultDto> ImportQuestionsAsync(ImportQuestionsRequestDto dto)
        {
            var result = new ImportQuestionsResultDto();
            var failedList = new List<ImportQuestionErrorDto>();
            int success = 0, failed = 0, total = 0;

            try
            {
                if (dto.File == null || dto.File.Length == 0)
                    throw ApiException.BadRequest("IMPORT_FILE_EMPTY", "No file uploaded.");

                List<ImportQuestionJsonDto>? questions;
                using (var stream = dto.File.OpenReadStream())
                {
                    questions = await JsonSerializer.DeserializeAsync<List<ImportQuestionJsonDto>>(stream, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }

                if (questions == null)
                    throw ApiException.BadRequest("IMPORT_FILE_INVALID", "File content is invalid or empty.");

                total = questions.Count;

                foreach (var q in questions)
                {
                    try
                    {
                        // Skip Listening questions
                        if (q.ContentName == ContentName.Listening)
                            throw new Exception("Importing Listening questions is not supported.");

                        // Validate required fields
                        if (string.IsNullOrWhiteSpace(q.Content) || q.Content.Length < 10)
                            throw new Exception("Question content is required and must be at least 10 characters.");
                        if (q.Points < 1 || q.Points > 100)
                            throw new Exception("Points must be between 1 and 100.");
                        if (q.Choices == null || q.Choices.Count < 2)
                            throw new Exception("At least 2 choices are required.");
                        if (!q.Choices.Any(c => c.IsCorrect))
                            throw new Exception("At least one correct choice is required.");

                        // Check for duplicate question
                        var isExisted = await _questionRepository.AnyAsync(x => x.questionText == q.Content);
                        if (isExisted)
                            throw new Exception("A question with the same text already exists.");

                        // Find SubContent
                        var subContent = await _subContentRepository.GetFirstOrDefaultAsync(
                            s => s.ContentName == q.ContentName
                              && s.Level == q.Level
                              && s.SubContentName == q.SubContentName
                        );
                        if (subContent == null)
                            throw new Exception("SubContent does not exist for the provided ContentName, Level, and SubContentName.");

                        // Create Question
                        var question = new Question
                        {
                            questionId = Guid.NewGuid(),
                            questionText = q.Content,
                            explanation = q.Explanation ?? string.Empty,
                            questionType = "multiple-choice",
                            points = q.Points,
                            difficulty = q.Difficulty,
                            SubContentId = subContent.SubContentId,
                            isActive = q.IsActive
                        };
                        await _questionRepository.InsertAsync(question);

                        // Create Choices
                        foreach (var c in q.Choices)
                        {
                            var choice = new Choice
                            {
                                choiceId = Guid.NewGuid(),
                                questionId = question.questionId,
                                choiceText = c.Content,
                                isCorrect = c.IsCorrect
                            };
                            await _choiceRepository.InsertAsync(choice);
                        }

                        await _questionRepository.SaveChangesAsync();
                        success++;
                    }
                    catch (Exception ex)
                    {
                        failed++;
                        failedList.Add(new ImportQuestionErrorDto
                        {
                            QuestionText = q.Content,
                            Error = ex.Message
                        });
                    }
                }

                // Write failedList to a file and return its path
                if (failedList.Count > 0)
                {
                    var failedJson = JsonSerializer.Serialize(failedList, new JsonSerializerOptions { WriteIndented = true });
                    var fileName = $"import_failed_{DateTime.UtcNow:yyyyMMddHHmmss}.json";
                    var filePath = Path.Combine(Path.GetTempPath(), fileName);
                    await File.WriteAllTextAsync(filePath, failedJson);
                    result.FailedFileUrl = fileName;
                }

                result.TotalCount = total;
                result.SuccessCount = success;
                result.FailedCount = failed;
                result.FailedQuestions = failedList;
                return result;
            }
            catch (ApiException) { throw; }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("IMPORT_QUESTIONS_ERROR", $"Error importing questions: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves random questions with choices for a specific subcontent.
        /// </summary>
        public async Task<List<RandomQuestionWithChoicesDto>> GetRandomQuestionsWithChoicesAsync(GetRandomQuestionsRequestDto requestDto)
        {
            try
            {
                // Find the SubContent entity matching the enums
                var subContent = await _subContentRepository.GetFirstOrDefaultAsync(
                    s => s.ContentName == requestDto.ContentName
                      && s.Level == requestDto.Level
                      && s.SubContentName == requestDto.SubContentName
                );
                if (subContent == null)
                    throw ApiException.BadRequest("SUBCONTENT_NOT_FOUND", "SubContent does not exist for the provided enums.");

                // Get random question IDs for this subcontent
                var questionIds = await _questionRepository.GetRandomQuestionIdsAsync(
                    subContent.SubContentId,
                    requestDto.NumberOfQuestions,
                    pointPerQuestion: 0 // or set as needed
                );
                if (questionIds == null || questionIds.Count == 0)
                    return new List<RandomQuestionWithChoicesDto>();

                // Load questions with choices
                var questions = await _questionRepository.GetAllAsync(
                    q => questionIds.Contains(q.questionId),
                    "Choices"
                );

                // Map to DTOs
                var result = questions.Select(q => new RandomQuestionWithChoicesDto
                {
                    QuestionId = q.questionId,
                    QuestionText = q.questionText,
                    Explanation = q.explanation,
                    Choices = q.Choices?.Select(c => new RandomChoiceDto
                    {
                        ChoiceId = c.choiceId,
                        Content = c.choiceText,
                        IsCorrect = c.isCorrect
                    }).ToList() ?? new List<RandomChoiceDto>()
                }).ToList();

                return result;
            }
            catch (ApiException) { throw; }
            catch (Exception ex)
            {
                // Catch and wrap unexpected errors
                throw ApiException.InternalServerError("GET_RANDOM_QUESTIONS_ERROR", ex.Message);
            }
        }

        // --- Helper for custom file name ---
        private static IFormFile CreateCustomFormFile(IFormFile originalFile, string customFileName)
        {
            var extension = Path.GetExtension(originalFile.FileName);
            var newFileName = customFileName + extension;
            return new CustomFormFile(originalFile, newFileName);
        }

        // --- CustomFormFile implementation ---
        internal class CustomFormFile : IFormFile
        {
            private readonly IFormFile _originalFile;
            private readonly string _customFileName;

            public CustomFormFile(IFormFile originalFile, string customFileName)
            {
                _originalFile = originalFile;
                _customFileName = customFileName;
            }

            public string ContentType => _originalFile.ContentType;
            public string ContentDisposition => _originalFile.ContentDisposition;
            public IHeaderDictionary Headers => _originalFile.Headers;
            public long Length => _originalFile.Length;
            public string Name => _originalFile.Name;
            public string FileName => _customFileName;

            public void CopyTo(Stream target) => _originalFile.CopyTo(target);
            public Task CopyToAsync(Stream target, CancellationToken cancellationToken = default) =>
                _originalFile.CopyToAsync(target, cancellationToken);
            public Stream OpenReadStream() => _originalFile.OpenReadStream();
        }

        /// <summary>
        /// Generates a new question using AI based on the specified parameters - Returns formatted content only, does not save to database
        /// </summary>
        public async Task<GeneratedQuestionResponseDto> GenerateQuestionWithAIAsync(GenerateQuestionRequestDto requestDto)
        {
            try
            {
                // 1. Generate question using AI (pass string parameters including description)
                var aiResult = await _aiIntegration.GenerateQuestionAsync(
                    requestDto.Level,
                    requestDto.ContentName,
                    requestDto.Description);

                if (!aiResult.IsValid || aiResult.Choices.Count != 4)
                    throw ApiException.InternalServerError("AI_GENERATION_FAILED", 
                        aiResult.ErrorMessage ?? "Failed to generate valid question from AI service");

                // 2. Validate AI response - must have exactly 1 correct answer
                var correctChoicesCount = aiResult.Choices.Count(c => c.IsCorrect);
                if (correctChoicesCount != 1)
                    throw ApiException.InternalServerError("AI_VALIDATION_FAILED", 
                        $"AI generated question must have exactly 1 correct answer, but got {correctChoicesCount}");

                // 3. Return formatted response (NO DATABASE SAVE) - Only QuestionText, Explanation and Choices
                return new GeneratedQuestionResponseDto
                {
                    QuestionText = aiResult.QuestionText,
                    Explanation = aiResult.Explanation,
                    Choices = aiResult.Choices.Select(c => new GeneratedChoiceDto
                    {
                        ChoiceText = c.Content,
                        IsCorrect = c.IsCorrect
                    }).ToList()
                };
            }
            catch (ApiException) { throw; }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("AI_QUESTION_SERVICE_ERROR", $"An error occurred while generating question with AI: {ex.Message}");
            }
        }

        /// <summary>
        /// Generates an explanation for a question using AI based on the question text and choices
        /// </summary>
        public async Task<ExplanationResponseDto> GenerateExplanationAsync(ExplanationRequestDto requestDto)
        {
            try
            {
                // Validate input
                if (string.IsNullOrEmpty(requestDto.QuestionText))
                    throw ApiException.BadRequest("INVALID_INPUT", "Question text cannot be empty");

                if (requestDto.Choices == null || !requestDto.Choices.Any())
                    throw ApiException.BadRequest("INVALID_INPUT", "Choices cannot be empty");

                var correctChoicesCount = requestDto.Choices.Count(c => c.IsCorrect);
                if (correctChoicesCount != 1)
                    throw ApiException.BadRequest("INVALID_INPUT", $"Question must have exactly 1 correct answer, but got {correctChoicesCount}");

                // Convert GeneratedChoiceDto to AIGeneratedChoice for the AI service
                var aiChoices = requestDto.Choices.Select(c => new Application.Contracts.AIGeneratedChoice
                {
                    Content = c.ChoiceText,
                    IsCorrect = c.IsCorrect
                }).ToList();

                // Generate explanation using AI
                var explanation = await _aiIntegration.GenerateExplanationAsync(
                    requestDto.QuestionText, 
                    aiChoices);

                return new ExplanationResponseDto
                {
                    Explanation = explanation
                };
            }
            catch (ApiException) { throw; }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("AI_EXPLANATION_SERVICE_ERROR", $"An error occurred while generating explanation with AI: {ex.Message}");
            }
        }
    }
}