using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.Domain.Entities
{
    public class Question
    {
        public Guid questionId { get; set; }
        public Guid SubContentId { get; set; }
        public string questionText { get; set; } = null!;
        public string questionType { get; set; } = null!;
        public string explanation { get; set; } = null!;
        public QuestionDifficulty difficulty { get; set; }

        public int points { get; set; }

        public virtual SubContent SubContent { get; set; } = null!;
        public virtual ICollection<TestQuestion> TestQuestions { get; set; } = new List<TestQuestion>();
        public virtual ICollection<Choice> Choices { get; set; } = new List<Choice>();
        public virtual ICollection<QuestionAttachment> QuestionAttachments { get; set; } = new List<QuestionAttachment>();
        public virtual ICollection<AttemptAnswer> AttemptAnswers { get; set; } = new List<AttemptAnswer>();
    }
}
