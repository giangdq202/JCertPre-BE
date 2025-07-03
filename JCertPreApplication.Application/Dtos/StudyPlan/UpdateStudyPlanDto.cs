using System.ComponentModel.DataAnnotations;

namespace JCertPreApplication.Application.Dtos.StudyPlan
{
    public class UpdateStudyPlanDto
    {
        [MinLength(3, ErrorMessage = "Plan name must be at least 3 characters")]
        public string? PlanName { get; set; }

        public string? Description { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public Guid? StudentId { get; set; }
    }
} 