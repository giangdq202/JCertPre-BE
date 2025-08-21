using System.ComponentModel.DataAnnotations;

namespace JCertPreApplication.Application.Dtos.TestTemplateConfig
{
    public class CreateTestTemplateConfigDto
    {
        [Required(ErrorMessage = "SubContentId is required.")]
        [NotDefaultGuid(ErrorMessage = "SubContentId must be a valid GUID.")]
        public Guid subContentId { get; set; }

        [Required(ErrorMessage = "Question count is required.")]
        [Range(1, 1000, ErrorMessage = "Question count must be between 1 and 1000.")]
        public int questionCount { get; set; }

        [Required(ErrorMessage = "Point per question is required.")]
        [Range(1, 100, ErrorMessage = "Point per question must be between 1 and 100.")]
        public int pointPerQuestion { get; set; }

        [Required(ErrorMessage = "Total points is required.")]
        [Range(1, 10000, ErrorMessage = "Total points must be between 1 and 10000.")]
        public int totalPoints { get; set; }

        [Required(ErrorMessage = "Sequence is required.")]
        [Range(1, 1000, ErrorMessage = "Sequence must be between 1 and 1000.")]
        public int sequence { get; set; }
    }
}