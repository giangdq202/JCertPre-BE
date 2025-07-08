using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.Domain.Entities
{
    public class Content
    {
        public int ContentId { get; set; }
        public ContentName ContentName { get; set; }

        // Navigation property
        public virtual ICollection<SubContent> SubContents { get; set; } = new List<SubContent>();
        public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
    }

    
}