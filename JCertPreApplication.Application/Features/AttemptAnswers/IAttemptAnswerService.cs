using JCertPreApplication.Application.Dtos.AttemptAnswer;

namespace JCertPreApplication.Application.Features.AttemptAnswers
{
    public interface IAttemptAnswerService
    {
        Task<List<AttemptAnswerDetailDto>> GetAllByAttemptIdAsync(Guid attemptId);
        Task<AttemptAnswerDetailDto> UpdateChoiceAsync(UpdateAttemptAnswerDto dto);
        Task<AttemptAnswerDetailDto> AddAnswerAsync(CreateAttemptAnswerDto dto);
    }
}