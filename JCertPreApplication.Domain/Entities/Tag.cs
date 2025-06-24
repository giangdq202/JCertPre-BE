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
        [Key]
        public Guid tagId { get; set; }

        [Required]
        [MaxLength(50)]
        public string tagLevel { get; set; }

        [Required]
        [MaxLength(100)]
        public string contentSection { get; set; }

        [Required]
        [MaxLength(1000)]
        public string contentDetail { get; set; }

        [Required]
        public int tagScore { get; set; }

        // Navigation properties
        public virtual ICollection<Question> Questions { get; set; }
    }
}
