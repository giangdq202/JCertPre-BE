using JCertPreApplication.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace JCertPreApplication.Application.Dtos.Course
{
    public class UpdateCourseDto
    {
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string? Title { get; set; }

        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
        public string? Description { get; set; }

        public CourseLevel? Level { get; set; }

        public CourseType? CourseType { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Price must be greater than or equal to 0")]
        public decimal? Price { get; set; }

        [Url(ErrorMessage = "Invalid URL format")]
        public string? ThumbnailUrl { get; set; }

        public string? Status { get; set; }
    }
} 