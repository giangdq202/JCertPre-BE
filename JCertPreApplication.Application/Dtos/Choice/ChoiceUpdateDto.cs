using System.ComponentModel.DataAnnotations;

namespace JCertPreApplication.Application.Dtos.Choice
{
    public class ChoiceUpdateDto
    {
        [MinLength(1, ErrorMessage = "Choice content cannot be empty.")]
        [MaxLength(500, ErrorMessage = "Choice content cannot exceed 500 characters.")]
        public string? Content { get; set; }

        public bool? IsCorrect { get; set; }
    }
}