using System;
using System.Collections.Generic;
using JCertPreApplication.Application.Dtos.Choice;
using JCertPreApplication.Application.Dtos.QuestionAttachment;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.Application.Dtos.Question
{
    public class QuestionDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = null!;
        public string? Explanation { get; set; }
        public int Points { get; set; }
        public QuestionDifficulty Difficulty { get; set; } // Add this line
        public ICollection<ChoiceReadDto>? Choices { get; set; }
        public ICollection<QuestionAttachmentDto>? QuestionAttachments { get; set; }

        // SubContent info
        public string ContentName { get; set; } = null!;
        public string ContentNameDescription { get; set; } = null!;
        public string Level { get; set; } = null!;
        public string LevelDescription { get; set; } = null!;
        public string SubContentName { get; set; } = null!;
        public string SubContentNameDescription { get; set; } = null!;
    }
}