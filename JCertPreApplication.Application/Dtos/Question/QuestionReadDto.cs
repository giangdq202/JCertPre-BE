namespace JCertPreApplication.Application.Dtos.Question
{
    public class QuestionReadDto
    {
        public Guid Id { get; set; }
        public required string Content { get; set; }
        public required string Type { get; set; }
        public int Points { get; set; }
        public string? Explanation { get; set; }
        public ICollection<Guid>? AttachmentIds { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
} 