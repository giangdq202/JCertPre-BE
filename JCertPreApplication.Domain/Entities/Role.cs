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
        public Guid roleId { get; set; }
        public string roleName { get; set; }
        public string description { get; set; }

        // Navigation property
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
