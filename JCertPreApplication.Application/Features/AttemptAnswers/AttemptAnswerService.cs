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
        private readonly ITestRepository _testRepository;

        public AttemptAnswerService(
            IAttemptAnswerRepository repo,
            ITestAttemptRepository testAttemptRepo,
            IChoiceRepository choiceRepo,
            IQuestionRepository questionRepo,
            ITestRepository testRepository)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _testAttemptRepo = testAttemptRepo ?? throw new ArgumentNullException(nameof(testAttemptRepo));
            _choiceRepo = choiceRepo ?? throw new ArgumentNullException(nameof(choiceRepo));
            _questionRepo = questionRepo ?? throw new ArgumentNullException(nameof(questionRepo));
            _testRepository = testRepository ?? throw new ArgumentNullException(nameof(testRepository));
        }

        /// <summary>
        /// Add or update one or multiple attempt answers. If answer exists, update choice; otherwise, add new.
        /// Throws if TestAttempt status is Suspended or Completed.
        /// </summary>
        public async Task<List<AttemptAnswerDetailDto>> AddOrUpdateAnswersAsync(
            IEnumerable<CreateAttemptAnswerDto> dtos,
            Guid userClaimId)
        {
            var result = new List<AttemptAnswerDetailDto>();
            try
            {
                foreach (var dto in dtos)
                {
                    var attempt = await _testAttemptRepo.GetByIdAsync(dto.AttemptId);
                    if (attempt == null)
                        throw ApiException.NotFound("TestAttempt", dto.AttemptId);

                    // Check if the attempt belongs to the current user
                    if (attempt.userId != userClaimId)
                        throw ApiException.Forbidden("FORBIDDEN", "You are not allowed to modify answers for this attempt.");

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

        /// <summary>
        /// Add or update one or multiple writing attempt answers.
        /// For writing tests, ChoiceId is always null and WrittenAnswer is required.
        /// Throws if TestAttempt status is Suspended or Completed.
        /// </summary>
        public async Task<List<WrittenAttemptAnswerDetailDto>> AddOrUpdateWritingAnswersAsync(
            IEnumerable<CreateWritingAttemptAnswerDto> dtos,
            Guid userClaimId)
        {
            var result = new List<WrittenAttemptAnswerDetailDto>();
            try
            {
                foreach (var dto in dtos)
                {
                    var attempt = await _testAttemptRepo.GetByIdAsync(dto.AttemptId);
                    if (attempt == null)
                        throw ApiException.NotFound("TestAttempt", dto.AttemptId);

                    if (attempt.userId != userClaimId)
                        throw ApiException.Forbidden("FORBIDDEN", "You are not allowed to modify answers for this attempt.");

                    if (attempt.status == TestAttemptStatus.Suspended)
                        throw ApiException.BadRequest("ATTEMPT_SUSPENDED", "Cannot add or update answer for a suspended test attempt.");
                    if (attempt.status == TestAttemptStatus.Completed)
                        throw ApiException.BadRequest("ATTEMPT_COMPLETED", "Cannot add or update answer for a completed test attempt.");

                    if (DateTime.UtcNow > attempt.endTime)
                        throw ApiException.BadRequest("ATTEMPT_OUT_OF_TIME", "Cannot add or update answer after test end time.");

                    var test = await _testRepository.GetByIdAsync(attempt.testId);
                    if (test == null)
                        throw ApiException.NotFound("Test", attempt.testId);
                    if (test.testType != TestType.WrittenManual)
                        throw ApiException.BadRequest("NOT_WRITING_TEST", "This function is only for writing tests.");

                    var question = await _questionRepo.GetByIdAsync(dto.QuestionId);
                    if (question == null)
                        throw ApiException.NotFound("Question", dto.QuestionId);

                    var existing = await _repo.GetFirstOrDefaultAsync(a => a.attemptId == dto.AttemptId && a.questionId == dto.QuestionId);
                    if (existing != null)
                    {
                        existing.WrittenAnswer = dto.WrittenAnswer;
                        existing.choiceId = null;
                        await _repo.UpdateAsync(existing);
                        result.Add(MapToWrittenDto(existing));
                    }
                    else
                    {
                        var answer = new AttemptAnswer
                        {
                            answerId = Guid.NewGuid(),
                            attemptId = dto.AttemptId,
                            questionId = dto.QuestionId,
                            choiceId = null,
                            WrittenAnswer = dto.WrittenAnswer,
                            isCorrect = false,
                            score = 0
                        };
                        await _repo.InsertAsync(answer);
                        result.Add(MapToWrittenDto(answer));
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
        /// Get all written attempt answers by attemptId.
        /// </summary>
        public async Task<List<WrittenAttemptAnswerDetailDto>> GetAllWrittenByAttemptIdAsync(Guid attemptId)
        {
            try
            {
                var answers = await _repo.GetAllAsync(a => a.attemptId == attemptId);
                return answers.Select(MapToWrittenDto).ToList();
            }
            catch (ApiException) { throw; }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("ATTEMPT_ANSWER_ERROR", ex.Message);
            }
        }

        /// <summary>
        /// Score a writing answer by answerId: update GraderComment, set isCorrect to true, and set score.
        /// Then, check if the answer's score meets the test's passing_percentage of the question's points.
        /// If so, set TestAttempt.isPass to true, else false.
        /// </summary>
        public async Task<WrittenAttemptAnswerDetailDto> ScoringWritingAsync(Guid answerId, int score, string graderComment)
        {
            try
            {
                // 1. Get the answer with its attempt and question
                var answer = await _repo.GetFirstOrDefaultAsync(
                    a => a.answerId == answerId,
                    "TestAttempt,Question"
                );
                if (answer == null)
                    throw ApiException.NotFound("AttemptAnswer", answerId);

                if (string.IsNullOrWhiteSpace(answer.WrittenAnswer))
                    throw ApiException.BadRequest("NOT_WRITING_ANSWER", "This answer is not a writing answer.");

                // 2. Update grading fields
                answer.GraderComment = graderComment;
                answer.isCorrect = true;
                answer.score = score;
                await _repo.UpdateAsync(answer);

                // 3. Get related TestAttempt and Test (fast, already included)
                var attempt = answer.TestAttempt;
                if (attempt == null)
                    attempt = await _testAttemptRepo.GetByIdAsync(answer.attemptId);
                if (attempt == null)
                    throw ApiException.NotFound("TestAttempt", answer.attemptId);

                var testId = attempt.testId;
                if (!testId.HasValue)
                    throw ApiException.BadRequest("NO_TEST_ID", "TestAttempt does not have a testId.");

                var test = attempt.Test;
                if (test == null)
                    test = await _testRepository.GetByIdAsync(testId.Value);
                if (test == null)
                    throw ApiException.NotFound("Test", testId.Value);

                // 4. Get the question (already included)
                var question = answer.Question;
                if (question == null)
                    question = await _questionRepo.GetByIdAsync(answer.questionId ?? Guid.Empty);
                if (question == null)
                    throw ApiException.NotFound("Question", answer.questionId ?? Guid.Empty);

                // 5. Calculate passing percentage
                // If answer.score >= question.points * (test.passing_percentage / 100), pass
                var requiredScore = (int)Math.Ceiling(question.points * (test.passing_percentage / 100m));
                attempt.isPass = answer.score >= requiredScore;

                await _testAttemptRepo.UpdateAsync(attempt);
                await _repo.SaveChangesAsync();
                await _testAttemptRepo.SaveChangesAsync();

                return new WrittenAttemptAnswerDetailDto
                {
                    AnswerId = answer.answerId,
                    AttemptId = answer.attemptId,
                    QuestionId = answer.questionId ?? Guid.Empty,
                    WrittenAnswer = answer.WrittenAnswer ?? string.Empty,
                    GraderComment = answer.GraderComment,
                    Score = answer.score
                };
            }
            catch (ApiException) { throw; }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("SCORING_WRITING_ERROR", ex.Message);
            }
        }

        private static AttemptAnswerDetailDto MapToDto(AttemptAnswer answer)
        {
            return new AttemptAnswerDetailDto
            {
                AnswerId = answer.answerId,
                AttemptId = answer.attemptId,
                QuestionId = answer.questionId ?? Guid.Empty,
                ChoiceId = answer.choiceId ?? Guid.Empty
            };
        }

        private static WrittenAttemptAnswerDetailDto MapToWrittenDto(AttemptAnswer answer)
        {
            return new WrittenAttemptAnswerDetailDto
            {
                AnswerId = answer.answerId,
                AttemptId = answer.attemptId,
                QuestionId = answer.questionId ?? Guid.Empty,
                WrittenAnswer = answer.WrittenAnswer ?? string.Empty,
                GraderComment = answer.GraderComment,
                Score = answer.score
            };
        }
    }
}