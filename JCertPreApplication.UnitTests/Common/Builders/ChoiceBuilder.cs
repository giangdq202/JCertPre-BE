using JCertPreApplication.Domain.Entities;

namespace JCertPreApplication.UnitTests.Common.Builders
{
    public class ChoiceBuilder
    {
        private Choice _choice;

        public ChoiceBuilder()
        {
            _choice = new Choice
            {
                choiceId = Guid.NewGuid(),
                questionId = Guid.NewGuid(),
                choiceText = "Sample choice text",
                isCorrect = false,
                AttemptAnswers = new List<AttemptAnswer>()
            };
        }

        public static ChoiceBuilder Create() => new ChoiceBuilder();

        public ChoiceBuilder WithId(Guid id)
        {
            _choice.choiceId = id;
            return this;
        }

        public ChoiceBuilder WithQuestionId(Guid questionId)
        {
            _choice.questionId = questionId;
            return this;
        }

        public ChoiceBuilder WithText(string text)
        {
            _choice.choiceText = text;
            return this;
        }

        public ChoiceBuilder AsCorrect()
        {
            _choice.isCorrect = true;
            return this;
        }

        public ChoiceBuilder AsIncorrect()
        {
            _choice.isCorrect = false;
            return this;
        }

        public ChoiceBuilder WithQuestion(Question question)
        {
            _choice.Question = question;
            _choice.questionId = question.questionId;
            return this;
        }

        public Choice Build() => _choice;
    }
}
