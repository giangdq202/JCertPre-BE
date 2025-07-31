using JCertPreApplication.Application.Dtos.Choice;

namespace JCertPreApplication.Application.Features.Choices
{
    public interface IChoiceService
    {
        Task<IEnumerable<ChoiceReadDto>> GetByQuestionIdAsync(Guid questionId);
        Task<ChoiceReadDto> CreateAsync(Guid questionId, ChoiceCreateDto dto);
        Task UpdateAsync(Guid choiceId, ChoiceUpdateDto dto); // Only needs ChoiceId
        Task DeleteAsync(Guid choiceId);       // Only needs ChoiceId
    }
}