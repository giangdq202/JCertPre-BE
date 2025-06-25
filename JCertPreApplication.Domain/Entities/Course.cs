using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.Domain.Entities
{
    //public class Course
    //{
    //    [Key]
    //    public Guid courseId { get; set; }

    //    [Required]
    //    [ForeignKey("User")]
    //    public Guid staffCreateUserId { get; set; }

    //    [Required]
    //    [MaxLength(100)]
    //    public string title { get; set; }

    //    [Required]
    //    [MaxLength(1000)]
    //    public string description { get; set; }

    //    [Required]
    //    public CourseLevel level { get; set; }

    //    [Required]
    //    public CourseType courseType { get; set; }

    //    [Required]
    //    public decimal price { get; set; }

    //    [Required]
    //    public string thumbnailUrl { get; set; }

    //    [Required]
    //    public string status { get; set; }

    //    [Required]
    //    public DateTime createdAt { get; set; }

    //    // Navigation properties
    //    public virtual User User { get; set; }
    //    public virtual ICollection<Lesson> Lessons { get; set; }
    //    public virtual ICollection<Livestream> Livestreams { get; set; }
    //    public virtual ICollection<Feedback> Feedbacks { get; set; }
    //    public virtual ICollection<Enrollment> Enrollments { get; set; }
    //}
    public class Course
    {
        public Guid courseId { get; set; }
        public Guid staffCreateUserId { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public CourseLevel level { get; set; }
        public CourseType courseType { get; set; }
        public decimal price { get; set; }
        public string thumbnailUrl { get; set; }
        public string status { get; set; }
        public DateTime createdAt { get; set; }

        // Navigation properties
        public virtual User User { get; set; }
        public virtual ICollection<Lesson> Lessons { get; set; }
        public virtual ICollection<Livestream> Livestreams { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<Enrollment> Enrollments { get; set; }
    }
}
