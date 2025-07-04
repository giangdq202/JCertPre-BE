using System;

namespace JCertPreApplication.Application.Dtos.Course
{
    public class CourseInstructorDto
    {
        public Guid CourseId { get; set; }
        public Guid InstructorId { get; set; }
        public string InstructorName { get; set; } = null!;
        public DateTime AssignedOn { get; set; }
        public DateTime? LeftOn { get; set; }
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
    }

    public class AssignInstructorToCourseDto
    {
        public Guid CourseId { get; set; }
        public Guid InstructorId { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdateCourseInstructorDto
    {
        public Guid CourseId { get; set; }
        public Guid InstructorId { get; set; }
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
    }
} 