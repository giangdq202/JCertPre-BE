using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.Domain.Entities
{
    public class Level
    {
        public int LevelId { get; set; }
        public LevelName LevelName { get; set; }

        // Navigation property
        public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
    }

    
}