using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.Domain.Entities
{
    public class SubContent
    {
        public int SubContentId { get; set; }
        public int ContentId { get; set; }
        public SubContentName SubContentName { get; set; }

        // Navigation property
        public virtual Content Content { get; set; } = null!;
        public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
    }
}
