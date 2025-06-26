namespace JCertPreApplication.Domain.Entities
{
    public class Role
    {
        public Guid roleId { get; set; }
        public string roleName { get; set; }
        public string description { get; set; }

        // Navigation property
        public virtual ICollection<User> Users { get; set; }
    }
}
