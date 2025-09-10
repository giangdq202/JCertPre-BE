using JCertPreApplication.Application.Dtos.TestAttempt;
using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.Application.Features.TestAttempts
{
    public interface ITestAttemptService
    {
        /// <summary>
        /// Start a test attempt for a user.
        /// </summary>
        Task<TestAttemptDto> StartTestAttemptAsync(StartTestAttemptDto dto);

        /// <summary>
        /// Submit a test attempt and calculate score.
        /// </summary>
        Task<TestAttemptDto> SubmitTestAttemptAsync(SubmitTestAttemptDto dto, Guid userClaimId);
        Task<TestAttemptDto> SubmitTestAttemptAsync(SubmitTestAttemptDto dto);
        /// <summary>
        /// Get all test attempts by user id.
        /// </summary>
        Task<List<TestAttemptDto>> GetAllByUserIdAsync(Guid userId);

        /// <summary>
        /// Update the status of a test attempt.
        /// </summary>
        Task<TestAttemptDto> UpdateStatusAsync(Guid attemptId, TestAttemptStatus status);

        Task<(TestAttemptDto Attempt, TestScoreSummary? ScoreSummary)> GetAttemptWithScoreSummaryAsync(Guid attemptId);

        /// <summary>
        /// Get paged test attempts by test id and isPass filter.
        /// </summary>
        Task<Pagination<TestAttemptDto>> GetPagedAttemptsByTestIdAndIsPassAsync(Guid testId, bool? isPass, int pageIndex = 1, int pageSize = 10);
    }
}