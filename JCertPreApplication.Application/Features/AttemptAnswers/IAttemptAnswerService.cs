using JCertPreApplication.Application.Dtos.AttemptAnswer;

namespace JCertPreApplication.Application.Features.AttemptAnswers
{
    public interface IAttemptAnswerService
    {
        Task<List<AttemptAnswerDetailDto>> AddOrUpdateAnswersAsync(IEnumerable<CreateAttemptAnswerDto> dtos, Guid userClaimId);
        Task<List<AttemptAnswerDetailDto>> GetAllByAttemptIdAsync(Guid attemptId);

        // Writing test support
        Task<List<WrittenAttemptAnswerDetailDto>> AddOrUpdateWritingAnswersAsync(IEnumerable<CreateWritingAttemptAnswerDto> dtos, Guid userClaimId);
        Task<List<WrittenAttemptAnswerDetailDto>> GetAllWrittenByAttemptIdAsync(Guid attemptId);

        /// <summary>
        /// Score a writing answer by answerId: update GraderComment, set isCorrect to true, and set score.
        /// Also updates TestAttempt.isPass based on passing percentage.
        /// </summary>
        Task<WrittenAttemptAnswerDetailDto> ScoringWritingAsync(Guid answerId, int score, string graderComment);
    }
}