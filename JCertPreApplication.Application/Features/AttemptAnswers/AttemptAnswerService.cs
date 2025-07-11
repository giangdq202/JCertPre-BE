using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.AttemptAnswer;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Domain.Entities;

namespace JCertPreApplication.Application.Features.AttemptAnswers
{
    public class AttemptAnswerService : IAttemptAnswerService
    {
        private readonly IAttemptAnswerRepository _repo;
        private readonly ITestAttemptRepository _testAttemptRepo;

        public AttemptAnswerService(
            IAttemptAnswerRepository repo,
            ITestAttemptRepository testAttemptRepo)
        {
            _repo = repo;
            _testAttemptRepo = testAttemptRepo;
        }

        /// <summary>
        /// Add a new attempt answer. Throws if out of time.
        /// </summary>
        public async Task<AttemptAnswerDetailDto> AddAnswerAsync(CreateAttemptAnswerDto dto)
        {
            try
            {
                var attempt = await _testAttemptRepo.GetByIdAsync(dto.AttemptId);
                if (attempt == null)
                    throw ApiException.NotFound("TestAttempt", dto.AttemptId);

                if (DateTime.UtcNow > attempt.endTime)
                    throw ApiException.BadRequest("ATTEMPT_OUT_OF_TIME", "Cannot add answer after test end time.");

                // Prevent duplicate questionId for this attempt
                var existing = await _repo.GetFirstOrDefaultAsync(a => a.attemptId == dto.AttemptId && a.questionId == dto.QuestionId);
                if (existing != null)
                    throw ApiException.BadRequest("DUPLICATE_QUESTION_ANSWER", "An answer for this question already exists in this attempt.");

                var answer = new AttemptAnswer
                {
                    answerId = Guid.NewGuid(),
                    attemptId = dto.AttemptId,
                    questionId = dto.QuestionId,
                    choiceId = dto.ChoiceId
                };
                await _repo.InsertAsync(answer);
                await _repo.SaveChangesAsync();
                return MapToDto(answer);
            }
            catch (ApiException) { throw; }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("ATTEMPT_ANSWER_ERROR", ex.Message);
            }
        }

        /// <summary>
        /// Update the choiceId for a specific answer. Throws if out of time.
        /// </summary>
        public async Task<AttemptAnswerDetailDto> UpdateChoiceAsync(UpdateAttemptAnswerDto dto)
        {
            try
            {
                var answer = await _repo.GetByIdAsync(dto.AnswerId);
                if (answer == null)
                    throw ApiException.NotFound("AttemptAnswer", dto.AnswerId);

                var attempt = await _testAttemptRepo.GetByIdAsync(answer.attemptId);
                if (attempt == null)
                    throw ApiException.NotFound("TestAttempt", answer.attemptId);

                if (DateTime.UtcNow > attempt.endTime)
                    throw ApiException.BadRequest("ATTEMPT_OUT_OF_TIME", "Cannot update answer after test end time.");

                answer.choiceId = dto.ChoiceId;
                await _repo.UpdateAsync(answer);
                await _repo.SaveChangesAsync();

                return MapToDto(answer);
            }
            catch (ApiException) { throw; }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("ATTEMPT_ANSWER_ERROR", ex.Message);
            }
        }

        /// <summary>
        /// Get all attempt answers by attemptId.
        /// </summary>
        public async Task<List<AttemptAnswerDetailDto>> GetAllByAttemptIdAsync(Guid attemptId)
        {
            try
            {
                var answers = await _repo.GetAllAsync(a => a.attemptId == attemptId);
                return answers.Select(MapToDto).ToList();
            }
            catch (ApiException) { throw; }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("ATTEMPT_ANSWER_ERROR", ex.Message);
            }
        }

        private static AttemptAnswerDetailDto MapToDto(AttemptAnswer answer)
        {
            return new AttemptAnswerDetailDto
            {
                AnswerId = answer.answerId,
                AttemptId = answer.attemptId,
                QuestionId = answer.questionId,
                ChoiceId = answer.choiceId
            };
        }
    }
}