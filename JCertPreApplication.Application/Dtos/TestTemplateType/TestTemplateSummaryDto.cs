using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JCertPreApplication.Application.Dtos.TestTemplateType
{
    public class TestTemplateSummaryDto
    {
        public Guid TemplateId { get; set; }
        public string TemplateName { get; set; } = string.Empty;
        public int TotalScore { get; set; }
        public decimal ToPassPercentage { get; set; }
        public int DurationMinutes { get; set; }
        public int TotalQuestionCount { get; set; }
    }
}
