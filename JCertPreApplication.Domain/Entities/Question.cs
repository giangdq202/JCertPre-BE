namespace JCertPreApplication.Domain.Entities
{
    public class Question
    {
        public Guid questionId { get; set; }
        public Guid SubContentId { get; set; }
        public string questionText { get; set; } = null!;
        public string questionType { get; set; } = null!;
        public string explanation { get; set; } = null!;
        public int points { get; set; }

        // Navigation properties
        public virtual SubContent SubContent { get; set; } = null!;
        public virtual ICollection<Test> Tests { get; set; } = new List<Test>();
        public virtual ICollection<Choice> Choices { get; set; } = new List<Choice>();
        public virtual ICollection<QuestionAttachment> QuestionAttachments { get; set; } = new List<QuestionAttachment>();
        public virtual ICollection<AttemptAnswer> AttemptAnswers { get; set; } = new List<AttemptAnswer>();
    }
}
