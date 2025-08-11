using System.ComponentModel.DataAnnotations;

namespace JCertPreApplication.Application.Dtos.Lesson
{
    public class UpdateLessonDto
    {
        [MinLength(1, ErrorMessage = "Lesson title cannot be empty.")]
        [MaxLength(200, ErrorMessage = "Lesson title cannot exceed 200 characters.")]
        public string? Title { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Lesson order must be a positive integer starting at 1.")]
        public int? LessonOrder { get; set; }

        [MinLength(1, ErrorMessage = "Lesson content cannot be empty.")]
        [MaxLength(5000, ErrorMessage = "Lesson content cannot exceed 5000 characters.")]
        public string? Content { get; set; }
    }
}
