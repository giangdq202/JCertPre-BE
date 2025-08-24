using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.UnitTests.Common.Builders
{
    public class QuestionBuilder
    {
        private Question _question;

        public QuestionBuilder()
        {
            _question = new Question
            {
                questionId = Guid.NewGuid(),
                SubContentId = Guid.NewGuid(),
                questionText = "Sample question text for testing purposes?",
                questionType = "multiple-choice",
                explanation = "This is a test explanation",
                difficulty = QuestionDifficulty.Easy,
                isActive = true,
                points = 5,
                Choices = new List<Choice>(),
                QuestionAttachments = new List<QuestionAttachment>(),
                AttemptAnswers = new List<AttemptAnswer>(),
                TestQuestions = new List<TestQuestion>()
            };
        }

        public static QuestionBuilder Create() => new QuestionBuilder();

        public QuestionBuilder WithId(Guid id)
        {
            _question.questionId = id;
            return this;
        }

        public QuestionBuilder WithSubContentId(Guid subContentId)
        {
            _question.SubContentId = subContentId;
            return this;
        }

        public QuestionBuilder WithContent(string content)
        {
            _question.questionText = content;
            return this;
        }

        public QuestionBuilder WithExplanation(string explanation)
        {
            _question.explanation = explanation;
            return this;
        }

        public QuestionBuilder WithPoints(int points)
        {
            _question.points = points;
            return this;
        }

        public QuestionBuilder WithDifficulty(QuestionDifficulty difficulty)
        {
            _question.difficulty = difficulty;
            return this;
        }

        public QuestionBuilder AsInactive()
        {
            _question.isActive = false;
            return this;
        }

        public QuestionBuilder WithChoices(ICollection<Choice> choices)
        {
            _question.Choices = choices;
            return this;
        }

        public QuestionBuilder WithSubContent(SubContent subContent)
        {
            _question.SubContent = subContent;
            _question.SubContentId = subContent.SubContentId;
            return this;
        }

        public QuestionBuilder WithQuestionAttachments(ICollection<QuestionAttachment> attachments)
        {
            _question.QuestionAttachments = attachments;
            return this;
        }

        public QuestionBuilder WithTestQuestions(ICollection<TestQuestion> testQuestions)
        {
            _question.TestQuestions = testQuestions;
            return this;
        }

        public QuestionBuilder WithAttemptAnswers(ICollection<AttemptAnswer> attemptAnswers)
        {
            _question.AttemptAnswers = attemptAnswers;
            return this;
        }

        public Question Build() => _question;
    }
}
