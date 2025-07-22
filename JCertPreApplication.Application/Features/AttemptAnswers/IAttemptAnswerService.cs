using JCertPreApplication.Application.Dtos.AttemptAnswer;

namespace JCertPreApplication.Application.Features.AttemptAnswers
{
    public interface IAttemptAnswerService
    {
        Task<List<AttemptAnswerDetailDto>> AddOrUpdateAnswersAsync(IEnumerable<CreateAttemptAnswerDto> dtos);
        Task<List<AttemptAnswerDetailDto>> GetAllByAttemptIdAsync(Guid attemptId);
    }
}