namespace JCertPreApplication.Application.Dtos.Question
{
    public class QuestionUpdateDto
    {
        public required string Content { get; set; }
        public required string Type { get; set; }
        public int Points { get; set; }
        public string? Explanation { get; set; }
        public ICollection<Guid>? AttachmentIds { get; set; }
    }
} 