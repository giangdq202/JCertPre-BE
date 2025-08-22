using System.ComponentModel.DataAnnotations;

namespace JCertPreApplication.Application.Dtos.Utilities
{
    // Custom validation attribute for non-default GUIDs
    public class NotDefaultGuidAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            return value is Guid guid && guid != Guid.Empty;
        }
    }
}
