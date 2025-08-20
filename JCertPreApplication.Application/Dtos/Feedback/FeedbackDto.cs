using System;

namespace JCertPreApplication.Application.Dtos.Feedback
{
    public class FeedbackDto
    {
        public Guid FeedbackId { get; set; }
        public Guid CourseId { get; set; }
        public Guid UserId { get; set; }
        public decimal Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? UserFullName { get; set; }      // Add this
        public string? UserAvatarUrl { get; set; }     // Add this
    }
}