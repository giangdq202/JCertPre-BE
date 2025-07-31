using JCertPreApplication.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace JCertPreApplication.Application.Dtos.Course
{
    /// <summary>
    /// 📝 Data transfer object for updating an existing course.
    /// </summary>
    /// <remarks>
    /// This DTO supports partial updates - only provide the fields you want to change.
    /// All fields are optional, but at least one field should be provided.
    /// </remarks>
    public class UpdateCourseDto
    {
        /// <summary>
        /// New course title. Must be unique if provided.
        /// </summary>
        /// <example>JLPT N5 Complete Course - Updated Edition</example>
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 200 characters")]
        public string? Title { get; set; }

        /// <summary>
        /// Updated course description.
        /// </summary>
        /// <example>Updated JLPT N5 preparation course with new interactive exercises and practice tests.</example>
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 2000 characters")]
        public string? Description { get; set; }

        /// <summary>
        /// Updated JLPT proficiency level (N5, N4, N3, N2, N1).
        /// </summary>
        /// <example>N4</example>
        public CourseLevel? Level { get; set; }

        /// <summary>
        /// Updated course delivery method (Online, Offline, Hybrid).
        /// </summary>
        /// <example>Hybrid</example>
        public CourseType? CourseType { get; set; }

        /// <summary>
        /// Updated course price in VND. Use 0 for free courses.
        /// </summary>
        /// <example>1800000</example>
        [Range(0, double.MaxValue, ErrorMessage = "Price must be greater than or equal to 0")]
        public decimal? Price { get; set; }

        /// <summary>
        /// Updated thumbnail image file. Provide to replace current thumbnail.
        /// Supported formats: JPEG, PNG, GIF, BMP, WebP, SVG
        /// </summary>
        /// <example>New image file upload</example>
        public IFormFile? ThumbnailFile { get; set; }

        /// <summary>
        /// Updated thumbnail URL. Set to null to remove current thumbnail.
        /// This field is used when not uploading a new file.
        /// </summary>
        /// <example>https://cdn.jcertpre.com/thumbnails/n5-course-updated-thumb.jpg</example>
        [Url(ErrorMessage = "Please provide a valid URL for the thumbnail")]
        public string? ThumbnailUrl { get; set; }

        /// <summary>
        /// Updated course status (Draft, Published, Archived, Suspended).
        /// </summary>
        /// <example>Published</example>
        public CourseStatus? Status { get; set; }
    }
} 