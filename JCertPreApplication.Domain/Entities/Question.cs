namespace JCertPreApplication.Domain.Entities
{
    public class Question
    {
        public Guid questionId { get; set; }
        public string questionText { get; set; }
        public string questionType { get; set; }
        public string explanation { get; set; }
        public Guid tagId { get; set; }

        // Navigation properties
        public virtual ICollection<Tag> Tag { get; set; }
        public virtual ICollection<Test> Tests { get; set; }
        public virtual ICollection<Choice> Choices { get; set; }
        public virtual ICollection<QuestionAttachment> QuestionAttachments { get; set; }
        public virtual ICollection<AttemptAnswer> AttemptAnswers { get; set; }
    }
}
