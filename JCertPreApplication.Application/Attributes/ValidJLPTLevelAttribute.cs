using System.ComponentModel.DataAnnotations;

namespace JCertPreApplication.Application.Attributes
{
    public class ValidJLPTLevelAttribute : ValidationAttribute
    {
        private static readonly string[] ValidLevels = { "N5", "N4", "N3", "N2", "N1" };

        public override bool IsValid(object? value)
        {
            if (value is string level)
            {
                return ValidLevels.Contains(level, StringComparer.OrdinalIgnoreCase);
            }
            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} must be one of: {string.Join(", ", ValidLevels)}";
        }
    }
}
