using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCertPreApplication.Domain.Entities
{
    public class Tag
    {
        public Guid tagId { get; set; }
        public string tagLevel { get; set; }
        public string contentSection { get; set; }
        public string contentDetail { get; set; }
        public int tagScore { get; set; }

        // Navigation properties
        public virtual ICollection<Question> Questions { get; set; }
    }
}
