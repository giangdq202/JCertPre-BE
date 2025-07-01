using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.Application.Dtos.Course
{
    public class CourseQueryParameters
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public Guid? InstructorId { get; set; }
        public CourseStatus? Status { get; set; }
        public CourseLevel? Level { get; set; }
        public CourseType? CourseType { get; set; }
        
        // Future extensions can be added here
        // public string? SortBy { get; set; }
        // public bool SortDescending { get; set; } = false;
    }
} 