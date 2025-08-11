using System;

namespace JCertPreApplication.Domain.Entities
{
    public class CourseInstructor
    {
        public Guid Id { get; set; } // Primary key for each assignment record
        
        public Guid CourseId { get; set; }
        public virtual Course Course { get; set; } = null!;

        public Guid InstructorId { get; set; }
        public virtual User Instructor { get; set; } = null!;

        public DateTime AssignedOn { get; set; }
        public DateTime? LeftOn { get; set; }
        public bool IsActive { get; set; }
        public string? Notes { get; set; }  // Có thể lưu lý do nghỉ hoặc ghi chú khác
    }
} 