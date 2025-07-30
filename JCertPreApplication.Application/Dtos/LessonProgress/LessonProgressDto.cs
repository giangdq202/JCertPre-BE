using System;

namespace JCertPreApplication.Application.Dtos.LessonProgress
{
    public class LessonProgressDto
    {
        public Guid ProgressId { get; set; }
        public Guid UserId { get; set; }
        public Guid LessonId { get; set; }
        public Guid CourseId { get; set; }
        public decimal CompletionRate { get; set; }
        public string? UserFullName { get; set; }
        public string? LessonTitle { get; set; }
    }
}