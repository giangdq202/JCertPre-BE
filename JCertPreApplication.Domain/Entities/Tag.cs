namespace JCertPreApplication.Domain.Entities
{
    public class Tag
    {
        public Guid tagId { get; set; }
        public string tagLevel { get; set; } = null!;
        public string contentSection { get; set; } = null!;
        public string contentDetail { get; set; } = null!;
        public int tagScore { get; set; }

        // Navigation properties
        public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
    }
}
