using System.ComponentModel.DataAnnotations;

namespace JCertPreApplication.Application.Dtos.LessonProgress
{
    public class CreateLessonProgressDto
    {
        [Required(ErrorMessage = "UserId is required.")]
        [NotDefaultGuid(ErrorMessage = "UserId must be a valid GUID.")]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "LessonId is required.")]
        [NotDefaultGuid(ErrorMessage = "LessonId must be a valid GUID.")]
        public Guid LessonId { get; set; }
    }

    
}