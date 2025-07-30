using System;

namespace JCertPreApplication.Application.Dtos.LessonProgress
{
    public class CreateLessonProgressDto
    {
        public Guid UserId { get; set; }
        public Guid LessonId { get; set; }
        public Guid CourseId { get; set; }
    }
}