using System;

namespace JCertPreApplication.Application.Dtos.TestTemplateConfig
{
    public class UpdateTestTemplateConfigDto
    {
        public int? questionCount { get; set; }
        public int? pointPerQuestion { get; set; }
        public int? totalPoints { get; set; }
        public int? sequence { get; set; }
    }
}