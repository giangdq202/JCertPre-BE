using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.AttemptAnswer;
using JCertPreApplication.Application.Features.AttemptAnswers;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;
using JCertPreApplication.UnitTests.Common.Builders;
using Moq;

namespace JCertPreApplication.UnitTests.Common.TestFixtures
{
    /// <summary>
    /// Test fixture for AttemptAnswerService providing mocked dependencies and helper methods
    /// </summary>
    public class AttemptAnswerServiceFixture
    {
        public AttemptAnswerService AttemptAnswerService { get; }
        public Mock<IAttemptAnswerRepository> MockAttemptAnswerRepository { get; }
        public Mock<ITestAttemptRepository> MockTestAttemptRepository { get; }
        public Mock<IChoiceRepository> MockChoiceRepository { get; }
        public Mock<IQuestionRepository> MockQuestionRepository { get; }
        public Mock<ITestRepository> MockTestRepository { get; }

        public AttemptAnswerServiceFixture()
        {
            MockAttemptAnswerRepository = new Mock<IAttemptAnswerRepository>();
            MockTestAttemptRepository = new Mock<ITestAttemptRepository>();
            MockChoiceRepository = new Mock<IChoiceRepository>();
            MockQuestionRepository = new Mock<IQuestionRepository>();
            MockTestRepository = new Mock<ITestRepository>();

            AttemptAnswerService = new AttemptAnswerService(
                MockAttemptAnswerRepository.Object,
                MockTestAttemptRepository.Object,
                MockChoiceRepository.Object,
                MockQuestionRepository.Object,
                MockTestRepository.Object
            );
        }

        /// <summary>
        /// Creates a valid CreateAttemptAnswerDto for testing
        /// </summary>
        public static CreateAttemptAnswerDto ValidCreateDto(
            Guid? attemptId = null,
            Guid? questionId = null,
            Guid? choiceId = null)
        {
            return new CreateAttemptAnswerDto
            {
                AttemptId = attemptId ?? Guid.NewGuid(),
                QuestionId = questionId ?? Guid.NewGuid(),
                ChoiceId = choiceId ?? Guid.NewGuid()
            };
        }

        /// <summary>
        /// Creates multiple CreateAttemptAnswerDto for batch testing
        /// </summary>
        public static List<CreateAttemptAnswerDto> CreateMultipleDtos(int count)
        {
            var dtos = new List<CreateAttemptAnswerDto>();
            for (int i = 0; i < count; i++)
            {
                dtos.Add(ValidCreateDto());
            }
            return dtos;
        }

        /// <summary>
        /// Creates a TestAttempt with InProgress status (valid for adding answers)
        /// </summary>
        public static TestAttempt CreateInProgressAttempt(Guid? attemptId = null)
        {
            var futureEndTime = DateTime.UtcNow.AddHours(1); // Not expired
            
            return TestAttemptBuilder.Create()
                .WithId(attemptId ?? Guid.NewGuid())
                .WithStatus(TestAttemptStatus.InProgress)
                .WithStartTime(DateTime.UtcNow.AddMinutes(-30))
                .WithEndTime(futureEndTime)
                .Build();
        }

        /// <summary>
        /// Creates a TestAttempt with Suspended status
        /// </summary>
        public static TestAttempt CreateSuspendedAttempt(Guid? attemptId = null)
        {
            return TestAttemptBuilder.Create()
                .WithId(attemptId ?? Guid.NewGuid())
                .WithStatus(TestAttemptStatus.Suspended)
                .WithStartTime(DateTime.UtcNow.AddMinutes(-30))
                .WithEndTime(DateTime.UtcNow.AddHours(1))
                .Build();
        }

        /// <summary>
        /// Creates a TestAttempt with Completed status
        /// </summary>
        public static TestAttempt CreateCompletedAttempt(Guid? attemptId = null)
        {
            return TestAttemptBuilder.Create()
                .WithId(attemptId ?? Guid.NewGuid())
                .WithStatus(TestAttemptStatus.Completed)
                .WithStartTime(DateTime.UtcNow.AddMinutes(-30))
                .WithEndTime(DateTime.UtcNow.AddMinutes(-5))
                .Build();
        }

        /// <summary>
        /// Creates a TestAttempt that has expired (endTime in the past)
        /// </summary>
        public static TestAttempt CreateExpiredAttempt(Guid? attemptId = null)
        {
            var pastEndTime = DateTime.UtcNow.AddMinutes(-10); // Expired 10 minutes ago
            
            return TestAttemptBuilder.Create()
                .WithId(attemptId ?? Guid.NewGuid())
                .WithStatus(TestAttemptStatus.InProgress)
                .WithStartTime(DateTime.UtcNow.AddHours(-2))
                .WithEndTime(pastEndTime)
                .Build();
        }

        /// <summary>
        /// Creates a Choice with isCorrect = true
        /// </summary>
        public static Choice CreateCorrectChoice(Guid? choiceId = null, Guid? questionId = null)
        {
            return ChoiceBuilder.Create()
                .WithId(choiceId ?? Guid.NewGuid())
                .WithQuestionId(questionId ?? Guid.NewGuid())
                .WithText("Correct Answer")
                .WithIsCorrect(true)
                .Build();
        }

        /// <summary>
        /// Creates a Choice with isCorrect = false
        /// </summary>
        public static Choice CreateIncorrectChoice(Guid? choiceId = null, Guid? questionId = null)
        {
            return ChoiceBuilder.Create()
                .WithId(choiceId ?? Guid.NewGuid())
                .WithQuestionId(questionId ?? Guid.NewGuid())
                .WithText("Incorrect Answer")
                .WithIsCorrect(false)
                .Build();
        }

        /// <summary>
        /// Creates a Question with specified points
        /// </summary>
        public static Question CreateQuestionWithPoints(int points, Guid? questionId = null)
        {
            return QuestionBuilder.Create()
                .WithId(questionId ?? Guid.NewGuid())
                .WithContent("Test Question")
                .WithPoints(points)
                .WithIsActive(true)
                .Build();
        }

        /// <summary>
        /// Creates an existing AttemptAnswer for update scenarios
        /// </summary>
        public static AttemptAnswer CreateExistingAnswer(
            Guid attemptId,
            Guid questionId,
            Guid? oldChoiceId = null,
            bool oldIsCorrect = false,
            int oldScore = 0)
        {
            return AttemptAnswerBuilder.Create()
                .WithAttemptId(attemptId)
                .WithQuestionId(questionId)
                .WithChoiceId(oldChoiceId ?? Guid.NewGuid())
                .WithIsCorrect(oldIsCorrect)
                .WithScore(oldScore)
                .Build();
        }

        /// <summary>
        /// Creates a list of AttemptAnswers for GetAllByAttemptId testing
        /// </summary>
        public static List<AttemptAnswer> CreateAnswersForAttempt(Guid attemptId, int count)
        {
            var answers = new List<AttemptAnswer>();
            for (int i = 0; i < count; i++)
            {
                var answer = AttemptAnswerBuilder.Create()
                    .WithAttemptId(attemptId)
                    .WithQuestionId(Guid.NewGuid())
                    .WithChoiceId(Guid.NewGuid())
                    .WithIsCorrect(i % 2 == 0) // Alternate correct/incorrect
                    .WithScore(i % 2 == 0 ? 5 : 0) // 5 points for correct, 0 for incorrect
                    .Build();
                answers.Add(answer);
            }
            return answers;
        }

        /// <summary>
        /// Creates DTOs with same attemptId and questionId for update testing
        /// </summary>
        public static (CreateAttemptAnswerDto dto, AttemptAnswer existing) CreateUpdateScenario()
        {
            var attemptId = Guid.NewGuid();
            var questionId = Guid.NewGuid();
            var oldChoiceId = Guid.NewGuid();
            var newChoiceId = Guid.NewGuid();

            var dto = ValidCreateDto(attemptId, questionId, newChoiceId);
            var existing = CreateExistingAnswer(attemptId, questionId, oldChoiceId, false, 0);

            return (dto, existing);
        }

        /// <summary>
        /// Creates DTOs for batch processing with mixed new and existing answers
        /// </summary>
        public static (List<CreateAttemptAnswerDto> dtos, List<AttemptAnswer> existingAnswers) CreateMixedBatchScenario()
        {
            var attemptId = Guid.NewGuid();
            var dtos = new List<CreateAttemptAnswerDto>();
            var existingAnswers = new List<AttemptAnswer>();

            // First DTO - new answer
            var newQuestionId = Guid.NewGuid();
            dtos.Add(ValidCreateDto(attemptId, newQuestionId, Guid.NewGuid()));

            // Second DTO - update existing answer
            var existingQuestionId = Guid.NewGuid();
            var updateDto = ValidCreateDto(attemptId, existingQuestionId, Guid.NewGuid());
            var existingAnswer = CreateExistingAnswer(attemptId, existingQuestionId);
            
            dtos.Add(updateDto);
            existingAnswers.Add(existingAnswer);

            return (dtos, existingAnswers);
        }
    }
}
