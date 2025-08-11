using System.ComponentModel.DataAnnotations;

namespace JCertPreApplication.Application.Dtos.LessonProgress
{
    public class UpdateLessonProgressDto
    {
        [Range(0, 100, ErrorMessage = "Completion rate must be between 0 and 100.")]
        public decimal CompletionRate { get; set; } // percent, 0-100
    }
}