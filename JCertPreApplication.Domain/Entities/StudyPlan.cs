namespace JCertPreApplication.Domain.Entities
{
    public class StudyPlan
    {
        public Guid planId { get; set; }
        public Guid studentId { get; set; }
        public Guid createdByStaffId { get; set; }
        public string planName { get; set; } = null!;
        public string description { get; set; } = null!;
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }

        // Navigation properties
        public virtual User Student { get; set; } = null!;
        public virtual User Staff { get; set; } = null!;
        public virtual ICollection<StudyPlanItem> StudyPlanItems { get; set; } = new List<StudyPlanItem>();
    }
}
