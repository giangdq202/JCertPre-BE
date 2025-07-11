using System;
using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.Application.Dtos.Test
{
    public class UpdateTestDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public TestType? TestType { get; set; }
        public int? DurationMinutes { get; set; }
        public DateTime? AvailableFrom { get; set; }
        public DateTime? AvailableTo { get; set; }
        public int? MaxAttempts { get; set; }
    }
}
