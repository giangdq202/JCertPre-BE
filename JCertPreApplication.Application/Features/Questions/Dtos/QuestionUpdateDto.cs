using System;
using System.ComponentModel.DataAnnotations;

namespace JCertPreApplication.Application.Features.Questions.Dtos
{
    public class QuestionUpdateDto
    {
        [Required]
        public Guid QuestionId { get; set; }

        [Required]
        [StringLength(500)]
        public string QuestionText { get; set; } = null!;

        [Required]
        public string QuestionType { get; set; } = null!;

        public string? Explanation { get; set; }

        [Required]
        public Guid TagId { get; set; }
    }
}