using System.ComponentModel.DataAnnotations;

namespace JCertPreApplication.Application.Dtos.TestAttempt;

public class StartTestAttemptDto
{
    [Required(ErrorMessage = "TestId is required.")]
    [NotDefaultGuid(ErrorMessage = "TestId must be a valid GUID.")]
    public Guid TestId { get; set; }

    [Required(ErrorMessage = "UserId is required.")]
    [NotDefaultGuid(ErrorMessage = "UserId must be a valid GUID.")]
    public Guid UserId { get; set; }
}