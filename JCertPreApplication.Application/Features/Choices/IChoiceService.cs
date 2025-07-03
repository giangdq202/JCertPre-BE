using JCertPreApplication.Application.Dtos.Choice;

namespace JCertPreApplication.Application.Features.Choices
{
    public interface IChoiceService
    {
        Task<IEnumerable<ChoiceReadDto>> GetByQuestionIdAsync(Guid questionId);
        Task<ChoiceReadDto> CreateAsync(Guid questionId, ChoiceCreateDto dto); // Added missing method definition  
        Task UpdateListAsync(Guid questionId, IEnumerable<ChoiceUpdateDto> dtos);
        Task DeleteAsync(Guid questionId, Guid choiceId);
    }
}