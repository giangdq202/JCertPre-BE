using JCertPreApplication.Application.Dtos.Utilities;
using System.ComponentModel.DataAnnotations;

namespace JCertPreApplication.Application.Dtos.TestQuestion;

public class AddTestQuestionManualDto
{
    [Required(ErrorMessage = "TestId is required.")]
    [NotDefaultGuid(ErrorMessage = "TestId must be a valid GUID.")]
    public Guid TestId { get; set; }

    [Required(ErrorMessage = "QuestionId is required.")]
    [NotDefaultGuid(ErrorMessage = "QuestionId must be a valid GUID.")]
    public Guid QuestionId { get; set; }
}