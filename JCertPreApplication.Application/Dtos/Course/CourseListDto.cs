using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.Application.Dtos.Course
{
    /// <summary>
    /// 📋 Simplified course information for list views and pagination.
    /// </summary>
    /// <remarks>
    /// This DTO contains essential course information optimized for list views, search results, 
    /// and pagination scenarios. It excludes detailed relationships to improve performance.
    /// </remarks>
    public class CourseListDto
    {
        /// <summary>
        /// Unique course identifier.
        /// </summary>
        /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
        public Guid CourseId { get; set; }

        /// <summary>
        /// Course title.
        /// </summary>
        /// <example>JLPT N5 Complete Course - Beginner Japanese</example>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Course description (may be truncated for list views).
        /// </summary>
        /// <example>Complete JLPT N5 preparation course covering hiragana, katakana, basic kanji...</example>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Target JLPT proficiency level.
        /// </summary>
        /// <example>N5</example>
        public CourseLevel Level { get; set; }

        /// <summary>
        /// Course delivery method.
        /// </summary>
        /// <example>Online</example>
        public CourseType CourseType { get; set; }

        /// <summary>
        /// Course price in VND. 0 indicates a free course.
        /// </summary>
        /// <example>1500000</example>
        public decimal Price { get; set; }

        /// <summary>
        /// URL to course thumbnail image. May be null if no thumbnail is set.
        /// </summary>
        /// <example>https://cdn.jcertpre.com/thumbnails/n5-course-thumb.jpg</example>
        public string? ThumbnailUrl { get; set; }

        /// <summary>
        /// Current course status.
        /// </summary>
        /// <example>Published</example>
        public CourseStatus Status { get; set; }

        /// <summary>
        /// Course creation timestamp (UTC).
        /// </summary>
        /// <example>2024-01-15T08:30:00Z</example>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Number of student enrollments (for popularity indication).
        /// </summary>
        /// <example>156</example>
        public int EnrollmentsCount { get; set; }

        /// <summary>
        /// Number of instructors teaching this course.
        /// </summary>
        /// <example>2</example>
        public int InstructorsCount { get; set; }
    }
} 