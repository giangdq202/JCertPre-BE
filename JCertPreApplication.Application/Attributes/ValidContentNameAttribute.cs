using System.ComponentModel.DataAnnotations;

namespace JCertPreApplication.Application.Attributes
{
    public class ValidContentNameAttribute : ValidationAttribute
    {
        private static readonly string[] ValidContentNames = { "Kanji", "Vocabulary", "Grammar", "Reading" };

        public override bool IsValid(object? value)
        {
            if (value is string contentName)
            {
                return ValidContentNames.Contains(contentName, StringComparer.OrdinalIgnoreCase);
            }
            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} must be one of: {string.Join(", ", ValidContentNames)}";
        }
    }
}
