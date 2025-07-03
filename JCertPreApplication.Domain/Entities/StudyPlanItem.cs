using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.Domain.Entities
{
    public class StudyPlanItem
    {
        public Guid itemId { get; set; }
        public Guid planId { get; set; }
        public int sequence { get; set; } //thu tu thuc hien trong ke hoach hoc tap
        public string itemType { get; set; } = null!;
        public Guid? courseId { get; set; }
        public Guid? testId { get; set; }
        public ItemStatus status { get; set; }

        // Navigation property
        public virtual StudyPlan StudyPlan { get; set; } = null!;
        public virtual Course Course { get; set; } = null!;
        public virtual Test Test { get; set; } = null!;
    }
}
