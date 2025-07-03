using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.Application.Dtos.StudyPlan
{
    public class UpdateStudyPlanItemDto
    {
        public int? Sequence { get; set; }
        public string? ItemType { get; set; }
        public Guid? CourseId { get; set; }
        public Guid? TestId { get; set; }
        public ItemStatus? Status { get; set; }
    }
} 