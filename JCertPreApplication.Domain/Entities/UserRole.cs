using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCertPreApplication.Domain.Entities
{
    public class UserRole
    {
        [Key, Column(Order = 0)]
        [ForeignKey("User")]
        public Guid userId { get; set; }

        [Key, Column(Order = 1)]
        [ForeignKey("Role")]
        public Guid roleId { get; set; }

        [Required]
        public DateTime assignedAt { get; set; }

        public virtual User User { get; set; }
        public virtual Role Role { get; set; }
    }
}
