namespace JCertPreApplication.Domain.Entities
{
    public class QuestionAttachment
    {
        public Guid attachmentId { get; set; }
        public Guid questionId { get; set; }
        public string mediaUrl { get; set; }
        public string mediaType { get; set; }

        // Navigation property
        public virtual Question Question { get; set; }
    }
}
