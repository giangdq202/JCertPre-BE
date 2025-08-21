using System.ComponentModel.DataAnnotations;
namespace JCertPreApplication.Application.Dtos.TestAttempt;
public class SubmitTestAttemptDto
{
    [Required(ErrorMessage = "AttemptId is required.")]
    [NotDefaultGuid(ErrorMessage = "AttemptId must be a valid GUID.")]
    public Guid AttemptId { get; set; }
}