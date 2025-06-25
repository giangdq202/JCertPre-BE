namespace JCertPreApplication.Domain.Entities
{
    public class Report
    {
        public Guid reportId { get; set; }
        public Guid reporterStudentId { get; set; }
        public Guid reportedInstructorId { get; set; }
        public string reportContent { get; set; }
        public string status { get; set; }
        public DateTime createdAt { get; set; }

        // Navigation properties
        public virtual User StudentUser { get; set; }
        public virtual User InstructorUser { get; set; }
    }
}
