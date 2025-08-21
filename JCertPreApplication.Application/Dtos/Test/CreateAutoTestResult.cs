using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.Application.Dtos.Test
{
    public class CreateAutoTestResult
    {
        public Guid TestId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
        public Guid TestTemplateTypeId { get; set; }
        public decimal PassingPercentage { get; set; }
        public TestStatus Status { get; set; }
    }
}
