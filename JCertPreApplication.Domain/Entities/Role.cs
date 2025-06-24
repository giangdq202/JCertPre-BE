using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCertPreApplication.Domain.Entities
{
    public class Role
    {
        [Key]
        public Guid roleId { get; set; }

        [Required]
        [StringLength(100)]
        public string roleName { get; set; }

        [StringLength(500)]
        public string description { get; set; }
        // Mối quan hệ nhiều-nhiều với User qua UserRole
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
