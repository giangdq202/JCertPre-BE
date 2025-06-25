using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

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
