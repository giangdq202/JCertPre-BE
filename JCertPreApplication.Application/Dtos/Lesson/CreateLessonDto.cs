using System.ComponentModel.DataAnnotations;

namespace JCertPreApplication.Application.Dtos.Lesson
{
    public class CreateLessonDto
    {
        [Required(ErrorMessage = "Lesson title is required.")]
        [MinLength(1, ErrorMessage = "Lesson title cannot be empty.")]
        [MaxLength(200, ErrorMessage = "Lesson title cannot exceed 200 characters.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lesson order is required.")]
        [Range(1, 10000, ErrorMessage = "Lesson order must be a positive integer between 1 and 10000.")]
        public int LessonOrder { get; set; }

        [Required(ErrorMessage = "Lesson content is required.")]
        [MinLength(1, ErrorMessage = "Lesson content cannot be empty.")]
        [MaxLength(5000, ErrorMessage = "Lesson content cannot exceed 5000 characters.")]
        public string Content { get; set; } = string.Empty;
    }
}
