namespace JCertPreApplication.Domain.Entities
{
    public class QuestionAttachment
    {
        public Guid attachmentId { get; set; }
        public Guid questionId { get; set; }
        public string mediaUrl { get; set; } = null!;
        public string mediaType { get; set; } = null!;

        // Navigation property
        public virtual Question Question { get; set; } = null!;
    }
}
