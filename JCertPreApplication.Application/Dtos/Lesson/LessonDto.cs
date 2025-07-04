using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCertPreApplication.Application.Dtos.Lesson
{
    public class LessonDto
    {
        public Guid LessonId { get; set; }
        public Guid CourseId { get; set; }
        public string Title { get; set; } = null!;
        public int LessonOrder { get; set; }
        public string Content { get; set; } = null!;
    }
}
