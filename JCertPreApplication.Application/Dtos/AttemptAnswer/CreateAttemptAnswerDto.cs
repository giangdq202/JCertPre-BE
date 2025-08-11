using System.ComponentModel.DataAnnotations;
using JCertPreApplication.Application.Dtos;

namespace JCertPreApplication.Application.Dtos.AttemptAnswer
{
    public class CreateAttemptAnswerDto
    {
        [Required(ErrorMessage = "AttemptId is required.")]
        [NotDefaultGuid(ErrorMessage = "AttemptId must be a valid GUID.")]
        public Guid AttemptId { get; set; }

        [Required(ErrorMessage = "QuestionId is required.")]
        [NotDefaultGuid(ErrorMessage = "QuestionId must be a valid GUID.")]
        public Guid QuestionId { get; set; }

        [Required(ErrorMessage = "ChoiceId is required.")]
        [NotDefaultGuid(ErrorMessage = "ChoiceId must be a valid GUID.")]
        public Guid ChoiceId { get; set; }
    }
}