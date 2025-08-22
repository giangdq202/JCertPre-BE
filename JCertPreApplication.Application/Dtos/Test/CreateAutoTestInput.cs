using System.ComponentModel.DataAnnotations;
using JCertPreApplication.Application.Dtos.Utilities;
using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.Application.Dtos.Test
{
    public class CreateAutoTestInput
    {
        [Required(ErrorMessage = "TestType is required.")]
        [AllowedTestType(ErrorMessage = "TestType must be either JLPTAuto or EntryAuto.")]
        public TestType TestType { get; set; }

        [Required(ErrorMessage = "CourseLevel is required.")]
        public CourseLevel CourseLevel { get; set; }
    }
}
