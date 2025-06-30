using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.Application.Dtos.Course
{
    public class CourseListDto
    {
        public Guid CourseId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public CourseLevel Level { get; set; }
        public CourseType CourseType { get; set; }
        public decimal Price { get; set; }
        public string ThumbnailUrl { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedByUserName { get; set; }
        public int EnrollmentsCount { get; set; }
    }
} 