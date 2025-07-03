namespace JCertPreApplication.Application.Dtos.StudyPlan
{
    public class StudyPlanDto
    {
        public Guid PlanId { get; set; }
        public Guid StudentId { get; set; }
        public Guid CreatedByStaffId { get; set; }
        public string PlanName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
