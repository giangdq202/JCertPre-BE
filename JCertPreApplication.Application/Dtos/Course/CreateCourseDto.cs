using JCertPreApplication.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace JCertPreApplication.Application.Dtos.Course
{
    public class CreateCourseDto
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Level is required")]
        public CourseLevel Level { get; set; }

        [Required(ErrorMessage = "Course type is required")]
        public CourseType CourseType { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be greater than or equal to 0")]
        public decimal Price { get; set; }

        [Url(ErrorMessage = "Invalid URL format")]
        public string? ThumbnailUrl { get; set; }
    }
} 