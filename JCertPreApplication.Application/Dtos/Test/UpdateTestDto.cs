using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCertPreApplication.Application.Dtos.Test
{
    public class UpdateTestDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? TestType { get; set; }
        public int? DurationMinutes { get; set; }
    }
}
