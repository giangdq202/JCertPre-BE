using JCertPreApplication.Application.Dtos.Choice;
using JCertPreApplication.Application.Dtos.QuestionAttachment;
using JCertPreApplication.Domain.Entities;

namespace JCertPreApplication.Application.Dtos.Question
{
    public class QuestionDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? Explanation { get; set; }
        public int Points { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public ICollection<ChoiceReadDto>? Choices { get; set; }
        public ICollection<QuestionAttachmentDto>? QuestionAttachments { get; set; }
    }
} 