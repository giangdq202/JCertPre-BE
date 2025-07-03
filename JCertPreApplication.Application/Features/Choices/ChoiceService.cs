using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Domain.Entities;

namespace JCertPreApplication.Application.Features.Choices
{
    public class ChoiceService : IChoiceService
    {
        private readonly IChoiceRepository _choiceRepo;

        public ChoiceService(IChoiceRepository choiceRepo)
        {
            _choiceRepo = choiceRepo;
        }

        public async Task<IEnumerable<ChoiceReadDto>> GetByQuestionIdAsync(Guid questionId)
        {
            var choices = await _choiceRepo.GetByQuestionIdAsync(questionId);
            return choices.Select(c => new ChoiceReadDto
            {
                ChoiceId = c.choiceId,
                ChoiceText = c.choiceText,
                IsCorrect = c.isCorrect
            });
        }

        public async Task<ChoiceReadDto> CreateAsync(Guid questionId, ChoiceCreateDto dto)
        {
            var choice = new Choice
            {
                choiceId = Guid.NewGuid(),
                questionId = questionId,
                choiceText = dto.ChoiceText,
                isCorrect = dto.IsCorrect
            };
            var created = await _choiceRepo.AddAsync(questionId, choice);
            return new ChoiceReadDto
            {
                ChoiceId = created.choiceId,
                ChoiceText = created.choiceText,
                IsCorrect = created.isCorrect
            };
        }

        public async Task UpdateListAsync(Guid questionId, IEnumerable<ChoiceUpdateDto> dtos)
        {
            var choices = dtos.Select(dto => new Choice
            {
                choiceId = dto.ChoiceId,
                questionId = questionId,
                choiceText = dto.ChoiceText,
                isCorrect = dto.IsCorrect
            });
            await _choiceRepo.UpdateListAsync(questionId, choices);
        }

        public async Task DeleteAsync(Guid questionId, Guid choiceId)
        {
            await _choiceRepo.DeleteAsync(questionId, choiceId);
        }
    }
}