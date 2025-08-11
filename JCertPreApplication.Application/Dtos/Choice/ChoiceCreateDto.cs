using System.ComponentModel.DataAnnotations;

namespace JCertPreApplication.Application.Dtos.Choice
{
    public class ChoiceCreateDto
    {
        [Required(ErrorMessage = "Choice content is required.")]
        [MinLength(1, ErrorMessage = "Choice content cannot be empty.")]
        [MaxLength(500, ErrorMessage = "Choice content cannot exceed 500 characters.")]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = "IsCorrect is required.")]
        public bool IsCorrect { get; set; }
    }
}