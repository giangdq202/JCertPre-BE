using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.Domain.Entities
{
    public class StudyPlanItem
    {
        public Guid itemId { get; set; }
        public Guid planId { get; set; }
        public int sequence { get; set; }
        public string itemType { get; set; } = null!;
        public Guid? courseId { get; set; }
        public Guid? TestTemplateTypeId { get; set; }
        public string? description { get; set; }
        public ItemStatus status { get; set; }

        // Navigation property
        public virtual StudyPlan StudyPlan { get; set; } = null!;
        public virtual Course? Course { get; set; }
        public virtual TestTemplateType? TestTemplateType { get; set; }
    }
}
