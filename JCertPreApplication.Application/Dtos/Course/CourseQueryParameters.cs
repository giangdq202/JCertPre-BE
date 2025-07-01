using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.Application.Dtos.Course
{
    /// <summary>
    /// 🔍 Query parameters for filtering, searching, and paginating courses.
    /// </summary>
    /// <remarks>
    /// This DTO consolidates all possible filtering options for courses, allowing flexible 
    /// and efficient querying. Multiple filters can be combined in a single request.
    /// </remarks>
    public class CourseQueryParameters
    {
        /// <summary>
        /// Page number for pagination (1-based indexing).
        /// </summary>
        /// <example>1</example>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Number of items per page. Maximum allowed is 100.
        /// </summary>
        /// <example>10</example>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// Search term to filter by course title or description (case-insensitive).
        /// </summary>
        /// <example>Japanese N5</example>
        public string? SearchTerm { get; set; }

        /// <summary>
        /// Filter courses by a specific instructor ID.
        /// </summary>
        /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
        public Guid? InstructorId { get; set; }

        /// <summary>
        /// Filter courses by status (Draft, Published, Archived, Suspended).
        /// </summary>
        /// <example>Published</example>
        public CourseStatus? Status { get; set; }

        /// <summary>
        /// Filter courses by JLPT level (N5, N4, N3, N2, N1).
        /// </summary>
        /// <example>N5</example>
        public CourseLevel? Level { get; set; }

        /// <summary>
        /// Filter courses by type (Online, Offline, Hybrid).
        /// </summary>
        /// <example>Online</example>
        public CourseType? CourseType { get; set; }
        
        // Future extensions can be added here
        // public string? SortBy { get; set; }
        // public bool SortDescending { get; set; } = false;
    }
} 