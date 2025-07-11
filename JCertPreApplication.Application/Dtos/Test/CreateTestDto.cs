using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.Application.Dtos.Test
{
    public class CreateTestDto
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public TestType TestType { get; set; }
        public int DurationMinutes { get; set; }
        public DateTime? AvailableFrom { get; set; }
        public DateTime? AvailableTo { get; set; }
        public int MaxAttempts { get; set; }
    }
}
