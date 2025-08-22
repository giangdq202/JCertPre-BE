using JCertPreApplication.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace JCertPreApplication.Application.Dtos.Utilities
{
    public class AllowedTestTypeAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is TestType testType)
            {
                return testType == TestType.JLPTAuto || testType == TestType.EntryAuto;
            }
            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} must be either JLPTAuto or EntryAuto.";
        }
    }
}
