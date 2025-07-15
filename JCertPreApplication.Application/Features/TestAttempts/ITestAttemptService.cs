using System.Collections.Generic;

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
        Task<TestAttemptDto> SubmitTestAttemptAsync(SubmitTestAttemptDto dto);

        /// <summary>
        /// Get all test attempts by user id.
        /// </summary>
        Task<List<TestAttemptDto>> GetAllByUserIdAsync(Guid userId);

        /// <summary>
        /// Update the status of a test attempt.
        /// </summary>
        Task<TestAttemptDto> UpdateStatusAsync(Guid attemptId, TestAttemptStatus status);
    }
}