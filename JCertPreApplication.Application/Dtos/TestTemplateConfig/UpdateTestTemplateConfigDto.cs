using System.ComponentModel.DataAnnotations;

namespace JCertPreApplication.Application.Dtos.TestTemplateConfig
{
    public class UpdateTestTemplateConfigDto
    {
        [Range(1, 1000, ErrorMessage = "Question count must be between 1 and 1000.")]
        public int? questionCount { get; set; }

        [Range(1, 100, ErrorMessage = "Point per question must be between 1 and 100.")]
        public int? pointPerQuestion { get; set; }

        [Range(1, 10000, ErrorMessage = "Total points must be between 1 and 10000.")]
        public int? totalPoints { get; set; }

        [Range(1, 1000, ErrorMessage = "Sequence must be between 1 and 1000.")]
        public int? sequence { get; set; }
    }
}