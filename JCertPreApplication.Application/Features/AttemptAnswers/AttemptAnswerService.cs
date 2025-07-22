using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.AttemptAnswer;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.Application.Features.AttemptAnswers
{
    public class AttemptAnswerService : IAttemptAnswerService
    {
        private readonly IAttemptAnswerRepository _repo;
        private readonly ITestAttemptRepository _testAttemptRepo;
        private readonly IChoiceRepository _choiceRepo;
        private readonly IQuestionRepository _questionRepo;

        public AttemptAnswerService(
            IAttemptAnswerRepository repo,
            ITestAttemptRepository testAttemptRepo,
            IChoiceRepository choiceRepo,
            IQuestionRepository questionRepo)
        {
            _repo = repo;
            _testAttemptRepo = testAttemptRepo;
            _choiceRepo = choiceRepo;
            _questionRepo = questionRepo;
        }

        /// <summary>
        /// Add or update one or multiple attempt answers. If answer exists, update choice; otherwise, add new.
        /// Throws if TestAttempt status is Suspended or Completed.
        /// </summary>
        public async Task<List<AttemptAnswerDetailDto>> AddOrUpdateAnswersAsync(IEnumerable<CreateAttemptAnswerDto> dtos)
        {
            var result = new List<AttemptAnswerDetailDto>();
            try
            {
                foreach (var dto in dtos)
                {
                    var attempt = await _testAttemptRepo.GetByIdAsync(dto.AttemptId);
                    if (attempt == null)
                        throw ApiException.NotFound("TestAttempt", dto.AttemptId);

                    // Check status before allowing add/update
                    if (attempt.status == TestAttemptStatus.Suspended)
                        throw ApiException.BadRequest("ATTEMPT_SUSPENDED", "Cannot add or update answer for a suspended test attempt.");
                    if (attempt.status == TestAttemptStatus.Completed)
                        throw ApiException.BadRequest("ATTEMPT_COMPLETED", "Cannot add or update answer for a completed test attempt.");

                    if (DateTime.UtcNow > attempt.endTime)
                        throw ApiException.BadRequest("ATTEMPT_OUT_OF_TIME", "Cannot add or update answer after test end time.");

                    var choice = await _choiceRepo.GetByIdAsync(dto.ChoiceId);
                    if (choice == null)
                        throw ApiException.NotFound("Choice", dto.ChoiceId);
                    var question = await _questionRepo.GetByIdAsync(dto.QuestionId);
                    if (question == null)
                        throw ApiException.NotFound("Question", dto.QuestionId);

                    // Check if answer exists for this attempt and question
                    var existing = await _repo.GetFirstOrDefaultAsync(a => a.attemptId == dto.AttemptId && a.questionId == dto.QuestionId);
                    if (existing != null)
                    {
                        // Update choice, isCorrect, score
                        existing.choiceId = dto.ChoiceId;
                        existing.isCorrect = choice.isCorrect;
                        existing.score = choice.isCorrect ? question.points : 0;
                        await _repo.UpdateAsync(existing);
                        result.Add(MapToDto(existing));
                    }
                    else
                    {
                        // Add new answer
                        var answer = new AttemptAnswer
                        {
                            answerId = Guid.NewGuid(),
                            attemptId = dto.AttemptId,
                            questionId = dto.QuestionId,
                            choiceId = dto.ChoiceId,
                            isCorrect = choice.isCorrect,
                            score = choice.isCorrect ? question.points : 0
                        };
                        await _repo.InsertAsync(answer);
                        result.Add(MapToDto(answer));
                    }
                }
                await _repo.SaveChangesAsync();
                return result;
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