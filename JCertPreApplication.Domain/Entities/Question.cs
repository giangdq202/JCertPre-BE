namespace JCertPreApplication.Domain.Entities
{
    public class Question
    {
        public Guid questionId { get; set; }
        public int? LevelId { get; set; }
        public int? ContentId { get; set; }
        public int? SubContentId { get; set; }
        public string questionText { get; set; } = null!;
        public string questionType { get; set; } = null!;
        public string explanation { get; set; } = null!;
        public int? points { get; set; }
        public string? GUID { get; set; }

        // Navigation properties
        public virtual Level? Level { get; set; }
        public virtual Content? Content { get; set; }
        public virtual SubContent? SubContent { get; set; }
        public virtual ICollection<Test> Tests { get; set; } = new List<Test>();
        public virtual ICollection<Choice> Choices { get; set; } = new List<Choice>();
        public virtual ICollection<QuestionAttachment> QuestionAttachments { get; set; } = new List<QuestionAttachment>();
        public virtual ICollection<AttemptAnswer> AttemptAnswers { get; set; } = new List<AttemptAnswer>();
    }
}
