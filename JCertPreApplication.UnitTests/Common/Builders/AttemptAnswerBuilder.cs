using JCertPreApplication.Domain.Entities;

namespace JCertPreApplication.UnitTests.Common.Builders
{
    /// <summary>
    /// Builder pattern for creating AttemptAnswer test data
    /// </summary>
    public class AttemptAnswerBuilder
    {
        private AttemptAnswer _attemptAnswer;

        public AttemptAnswerBuilder()
        {
            _attemptAnswer = new AttemptAnswer
            {
                answerId = Guid.NewGuid(),
                attemptId = Guid.NewGuid(),
                questionId = Guid.NewGuid(),
                choiceId = Guid.NewGuid(),
                isCorrect = false,
                score = 0
            };
        }

        public static AttemptAnswerBuilder Create() => new AttemptAnswerBuilder();

        public AttemptAnswerBuilder WithId(Guid id)
        {
            _attemptAnswer.answerId = id;
            return this;
        }

        public AttemptAnswerBuilder WithAttemptId(Guid attemptId)
        {
            _attemptAnswer.attemptId = attemptId;
            return this;
        }

        public AttemptAnswerBuilder WithQuestionId(Guid questionId)
        {
            _attemptAnswer.questionId = questionId;
            return this;
        }

        public AttemptAnswerBuilder WithChoiceId(Guid choiceId)
        {
            _attemptAnswer.choiceId = choiceId;
            return this;
        }

        public AttemptAnswerBuilder WithIsCorrect(bool isCorrect)
        {
            _attemptAnswer.isCorrect = isCorrect;
            return this;
        }

        public AttemptAnswerBuilder WithScore(int score)
        {
            _attemptAnswer.score = score;
            return this;
        }

        public AttemptAnswerBuilder WithTestAttempt(TestAttempt testAttempt)
        {
            _attemptAnswer.TestAttempt = testAttempt;
            return this;
        }

        public AttemptAnswerBuilder WithQuestion(Question question)
        {
            _attemptAnswer.Question = question;
            return this;
        }

        public AttemptAnswerBuilder WithChoice(Choice choice)
        {
            _attemptAnswer.Choice = choice;
            return this;
        }

        public AttemptAnswer Build() => _attemptAnswer;
    }
}
