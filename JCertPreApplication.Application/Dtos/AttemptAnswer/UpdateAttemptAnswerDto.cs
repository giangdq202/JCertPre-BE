using JCertPreApplication.Application.Dtos.Utilities;
using System.ComponentModel.DataAnnotations;

namespace JCertPreApplication.Application.Dtos.AttemptAnswer
{
    public class UpdateAttemptAnswerDto
    {
        [Required(ErrorMessage = "AnswerId is required.")]
        [NotDefaultGuid(ErrorMessage = "AnswerId must be a valid GUID.")]
        public Guid AnswerId { get; set; }

        [Required(ErrorMessage = "ChoiceId is required.")]
        [NotDefaultGuid(ErrorMessage = "ChoiceId must be a valid GUID.")]
        public Guid ChoiceId { get; set; }
    }
}