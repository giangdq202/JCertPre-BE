using System.ComponentModel.DataAnnotations;

namespace JCertPreApplication.Application.Dtos
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
