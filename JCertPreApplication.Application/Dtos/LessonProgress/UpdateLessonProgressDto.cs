using System;

namespace JCertPreApplication.Application.Dtos.LessonProgress
{
    public class UpdateLessonProgressDto
    {
        public decimal CompletionRate { get; set; } // percent, 0-100
    }
}