using System.ComponentModel.DataAnnotations;
using JCertPreApplication.Application.Dtos;

public class SubmitTestAttemptDto
{
    [Required(ErrorMessage = "AttemptId is required.")]
    [NotDefaultGuid(ErrorMessage = "AttemptId must be a valid GUID.")]
    public Guid AttemptId { get; set; }
}