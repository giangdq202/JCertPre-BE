using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Application.Dtos.Choice;

namespace JCertPreApplication.Application.Features.Choices
{
    public class ChoiceService : IChoiceService
    {
        private readonly IChoiceRepository _choiceRepo;

        public ChoiceService(IChoiceRepository choiceRepo)
        {
            _choiceRepo = choiceRepo ?? throw new ArgumentNullException(nameof(choiceRepo));
        }

        public async Task<IEnumerable<ChoiceReadDto>> GetByQuestionIdAsync(Guid questionId)
        {
            try
            {
                var choices = await _choiceRepo.GetByQuestionIdAsync(questionId);
                return choices.Select(c => new ChoiceReadDto
                {
                    Id = c.choiceId,
                    Content = c.choiceText,
                    IsCorrect = c.isCorrect,
                    QuestionId = c.questionId
                });
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("CHOICE_SERVICE_ERROR", $"An error occurred while retrieving choices: {ex.Message}");
            }
        }

        public async Task<ChoiceReadDto> CreateAsync(Guid questionId, ChoiceCreateDto dto)
        {
            try
            {
                var choice = new Choice
                {
                    choiceId = Guid.NewGuid(),
                    questionId = questionId,
                    choiceText = dto.Content,
                    isCorrect = dto.IsCorrect
                };
                var created = await _choiceRepo.AddAsync(questionId, choice);
                return new ChoiceReadDto
                {
                    Id = created.choiceId,
                    Content = created.choiceText,
                    IsCorrect = created.isCorrect,
                    QuestionId = created.questionId
                };
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("CHOICE_CREATE_ERROR", $"An error occurred while creating choice: {ex.Message}");
            }
        }

        public async Task UpdateListAsync(Guid questionId, IEnumerable<ChoiceUpdateDto> dtos)
        {
            try
            {
                var choices = dtos.Select(dto => new Choice
                {
                    choiceId = Guid.NewGuid(), // For update list, we create new choices
                    questionId = questionId,
                    choiceText = dto.Content,
                    isCorrect = dto.IsCorrect
                });
                await _choiceRepo.UpdateListAsync(questionId, choices);
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("CHOICE_UPDATE_ERROR", $"An error occurred while updating choices: {ex.Message}");
            }
        }

        public async Task DeleteAsync(Guid questionId, Guid choiceId)
        {
            try
            {
                await _choiceRepo.DeleteAsync(questionId, choiceId);
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("CHOICE_DELETE_ERROR", $"An error occurred while deleting choice: {ex.Message}");
            }
        }
    }
}