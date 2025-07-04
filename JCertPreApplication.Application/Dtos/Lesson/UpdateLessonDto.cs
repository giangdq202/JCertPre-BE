using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCertPreApplication.Application.Dtos.Lesson
{
    public class UpdateLessonDto
    {
        public string? Title { get; set; }
        public int? LessonOrder { get; set; }
        public string? Content { get; set; }
    }
}
