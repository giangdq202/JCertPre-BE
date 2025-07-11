namespace JCertPreApplication.Application.Dtos.Enrollment
{
    public class EnrollmentResponseDto
    {
        public Guid EnrollmentId { get; set; }
        public Guid UserId { get; set; }
        public Guid CourseId { get; set; }
        public string CourseTitle { get; set; } = null!;
        public decimal PricePaid { get; set; }
        public DateTime EnrollDate { get; set; }
        public int RemainingCredit { get; set; }
        public string Message { get; set; } = null!;
    }
} 