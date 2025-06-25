using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCertPreApplication.Domain.Entities
{
    public class Livestream
    {
        public Guid livestreamId { get; set; }
        public Guid courseId { get; set; }
        public string title { get; set; }
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
        public string meetingUrl { get; set; }
        public string recordingUrl { get; set; }

        // Navigation property
        public virtual Course Course { get; set; }
    }
}
