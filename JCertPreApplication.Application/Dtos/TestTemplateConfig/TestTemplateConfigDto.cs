using System;

namespace JCertPreApplication.Application.Dtos.TestTemplateConfig
{
    public class TestTemplateConfigDto
    {
        public Guid configId { get; set; }
        public Guid templateId { get; set; }
        public Guid subContentId { get; set; }
        public int questionCount { get; set; }
        public int pointPerQuestion { get; set; }
        public int totalPoints { get; set; }
        public int sequence { get; set; }
        public string? subContentName { get; set; }
        public SubContentDto? SubContent { get; set; }
    }
}