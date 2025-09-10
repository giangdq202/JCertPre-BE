using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Application.Dtos.Choice;
using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.Application.Features.Choices
{
    /// <summary>
    /// Service for handling business logic related to Choice entities.
    /// Implements exception handling and follows Clean Architecture best practices.
    /// </summary>
    public class ChoiceService : IChoiceService
    {
        private readonly IChoiceRepository _choiceRepo;
        private readonly IQuestionRepository _questionRepository;

        public ChoiceService(IChoiceRepository choiceRepo, IQuestionRepository questionRepository)
        {
            _choiceRepo = choiceRepo ?? throw new ArgumentNullException(nameof(choiceRepo));
            _questionRepository = questionRepository ?? throw new ArgumentNullException(nameof(questionRepository));
        }

        /// <summary>
        /// Retrieves all choices for a question.
        /// </summary>
        public async Task<IEnumerable<ChoiceReadDto>> GetByQuestionIdAsync(Guid questionId)
        {
            try
            {
                var choices = await _choiceRepo.GetAllAsync(c => c.questionId == questionId);
                return choices.Select(MapToChoiceReadDto);
            }
            catch (ApiException) { throw; }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("CHOICE_SERVICE_ERROR", $"An error occurred while retrieving choices: {ex.Message}");
            }
        }

        /// <summary>
        /// Creates a new choice for a question.
        /// Prevents creating choices for questions with content name 'Writing'.
        /// </summary>
        public async Task<ChoiceReadDto> CreateAsync(Guid questionId, ChoiceCreateDto dto)
        {
            try
            {
                // Efficiently fetch the question and its SubContent's ContentName in one query
                var question = await _questionRepository.GetFirstOrDefaultAsync(
                    q => q.questionId == questionId,
                    "SubContent"
                );

                if (question == null)
                    throw ApiException.NotFound("Question", questionId);

                // Fast check: block choices for Writing questions
                if (question.SubContent.ContentName == ContentName.Writing)
                    throw ApiException.BadRequest("CHOICE_NOT_ALLOWED", "Cannot create choices for questions with content name 'Writing'.");

                // Get current number of choices for the question
                var choiceCount = await _choiceRepo.GetAllAsync(c => c.questionId == questionId);
                if (choiceCount.Count >= 4)
                    throw ApiException.BadRequest("CHOICE_LIMIT", "A question can only have 4 choices.");

                var choice = new Choice
                {
                    choiceId = Guid.NewGuid(),
                    questionId = questionId,
                    choiceText = dto.Content,
                    isCorrect = dto.IsCorrect
                };
                var created = await _choiceRepo.InsertAsync(choice);
                await _choiceRepo.SaveChangesAsync();

                // Concurrency safety: check again after insert
                var totalChoices = await _choiceRepo.GetAllAsync(c => c.questionId == questionId);
                if (totalChoices.Count > 4)
                {
                    await _choiceRepo.DeleteAsync(created);
                    await _choiceRepo.SaveChangesAsync();
                    throw ApiException.BadRequest("CHOICE_LIMIT", "A question can only have 4 choices.");
                }

                return MapToChoiceReadDto(created);
            }
            catch (ApiException) { throw; }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("CHOICE_CREATE_ERROR", $"An error occurred while creating choice: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates a choice by its ID and question ID.
        /// /// </summary>
        public async Task UpdateAsync(Guid choiceId, ChoiceUpdateDto dto)
        {
            try
            {
                var choice = await _choiceRepo.GetByIdAsync(choiceId);
                if (choice == null)
                    throw ApiException.NotFound("Choice", choiceId);

                if (dto.Content != null)
                    choice.choiceText = dto.Content;
                if (dto.IsCorrect.HasValue)
                    choice.isCorrect = dto.IsCorrect.Value;

                await _choiceRepo.UpdateAsync(choice);
                await _choiceRepo.SaveChangesAsync();
            }
            catch (ApiException) { throw; }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("CHOICE_UPDATE_ERROR", $"An error occurred while updating choice: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes a choice by its ID and question ID.
        /// </summary>
        public async Task DeleteAsync(Guid choiceId)
        {
            try
            {
                var choice = await _choiceRepo.GetByIdAsync(choiceId);
                if (choice == null)
                    throw ApiException.NotFound("Choice", choiceId);

                await _choiceRepo.DeleteAsync(choice);
                await _choiceRepo.SaveChangesAsync();
            }
            catch (ApiException) { throw; }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("CHOICE_DELETE_ERROR", $"An error occurred while deleting choice: {ex.Message}");
            }
        }

        /// <summary>
        /// Maps a Choice entity to a ChoiceReadDto.
        /// </summary>
        private static ChoiceReadDto MapToChoiceReadDto(Choice c)
        {
            return new ChoiceReadDto
            {
                ChoiceId = c.choiceId,
                QuestionId = c.questionId,
                Content = c.choiceText,
                IsCorrect = c.isCorrect
            };
        }
    }
}