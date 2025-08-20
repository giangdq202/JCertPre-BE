using System.ComponentModel.DataAnnotations;

namespace JCertPreApplication.Application.Dtos.Question
{
    public class GenerateQuestionRequestDto : IValidatableObject
    {
        [Required(ErrorMessage = "JLPT Level is required.")]
        public string Level { get; set; } = string.Empty;

        [Required(ErrorMessage = "Content name is required.")]
        public string ContentName { get; set; } = string.Empty;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            // Validate Level
            var validLevels = new[] { "N5", "N4", "N3", "N2", "N1" };
            if (!validLevels.Contains(Level, StringComparer.OrdinalIgnoreCase))
            {
                results.Add(new ValidationResult(
                    $"Level must be one of: {string.Join(", ", validLevels)}", 
                    new[] { nameof(Level) }));
            }

            // Validate ContentName
            var validContentNames = new[] { "Kanji", "Vocabulary", "Grammar", "Reading" };
            if (!validContentNames.Contains(ContentName, StringComparer.OrdinalIgnoreCase))
            {
                results.Add(new ValidationResult(
                    $"Content name must be one of: {string.Join(", ", validContentNames)}", 
                    new[] { nameof(ContentName) }));
            }

            return results;
        }
    }
}
