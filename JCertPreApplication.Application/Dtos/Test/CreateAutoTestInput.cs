using System.ComponentModel.DataAnnotations;
using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.Application.Dtos.Test
{
    public class CreateAutoTestInput
    {
        [Required(ErrorMessage = "TestType is required.")]
        public TestType TestType { get; set; }

        [Required(ErrorMessage = "CourseLevel is required.")]
        public CourseLevel CourseLevel { get; set; }
    }
}
