using JCertPreApplication.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace JCertPreApplication.Application.Dtos.Course
{
    /// <summary>
    /// Data transfer object for creating a new course.
    /// </summary>
    /// <remarks>
    /// This DTO contains all required information to create a new course. 
    /// The course will be created with Draft status by default.
    /// </remarks>
    public class CreateCourseDto
    {
        /// <summary>
        /// Course title. Must be unique across all courses.
        /// </summary>
        /// <example>JLPT N5 Complete Course - Beginner Japanese</example>
        [Required(ErrorMessage = "Course title is required")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 200 characters")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Detailed description of the course content and objectives.
        /// </summary>
        /// <example>Complete JLPT N5 preparation course covering hiragana, katakana, basic kanji (100 characters), essential grammar patterns, and vocabulary (800+ words). Perfect for absolute beginners.</example>
        [Required(ErrorMessage = "Course description is required")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 2000 characters")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// JLPT proficiency level that this course targets (N5, N4, N3, N2, N1).
        /// </summary>
        /// <example>N5</example>
        [Required(ErrorMessage = "Course level is required")]
        public CourseLevel Level { get; set; }

        /// <summary>
        /// Type of course delivery method (Online, Offline, Hybrid).
        /// </summary>
        /// <example>Online</example>
        [Required(ErrorMessage = "Course type is required")]
        public CourseType CourseType { get; set; }

        /// <summary>
        /// Course price in VND. Set to 0 for free courses.
        /// </summary>
        /// <example>1500000</example>
        [Range(0, double.MaxValue, ErrorMessage = "Price must be greater than or equal to 0")]
        public decimal Price { get; set; }

        /// <summary>
        /// URL to the course thumbnail image. Optional field.
        /// </summary>
        /// <example>https://cdn.jcertpre.com/thumbnails/n5-course-thumb.jpg</example>
        [Url(ErrorMessage = "Please provide a valid URL for the thumbnail")]
        public string? ThumbnailUrl { get; set; }
    }
} 