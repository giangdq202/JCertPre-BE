using System.ComponentModel.DataAnnotations;
using JCertPreApplication.Domain.Enums;
namespace JCertPreApplication.Application.Dtos.Question;
public class GetRandomQuestionsRequestDto : IValidatableObject
{
    [Range(5, 20, ErrorMessage = "NumberOfQuestions must be between 5 and 20.")]
    public int NumberOfQuestions { get; set; }

    [Required]
    public ContentName ContentName { get; set; }

    [Required]
    public CourseLevel Level { get; set; }

    [Required]
    public SubContentName SubContentName { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!Enum.IsDefined(typeof(ContentName), ContentName))
            yield return new ValidationResult("Invalid ContentName.", new[] { nameof(ContentName) });
        if (!Enum.IsDefined(typeof(CourseLevel), Level))
            yield return new ValidationResult("Invalid Level.", new[] { nameof(Level) });
        if (!Enum.IsDefined(typeof(SubContentName), SubContentName))
            yield return new ValidationResult("Invalid SubContentName.", new[] { nameof(SubContentName) });
    }
}