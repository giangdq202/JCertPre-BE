using System;

namespace JCertPreApplication.Application.Dtos.TestTemplateConfig
{
    public class CreateTestTemplateConfigDto
    {
        public Guid subContentId { get; set; }
        public int questionCount { get; set; }
        public int pointPerQuestion { get; set; }
        public int totalPoints { get; set; }
        public int sequence { get; set; }
    }
}