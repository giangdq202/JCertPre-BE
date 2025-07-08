using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCertPreApplication.Application.Dtos.Test
{
    public class CreateTestDto
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string TestType { get; set; } = null!;
        public int DurationMinutes { get; set; }
    }
}
