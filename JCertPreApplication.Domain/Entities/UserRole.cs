namespace JCertPreApplication.Domain.Entities
{
    public class UserRole
    {
        public Guid userId { get; set; }
        public Guid roleId { get; set; }
        public DateTime assignedAt { get; set; }

        public virtual User User { get; set; }
        public virtual Role Role { get; set; }
    }
}
