using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCertPreApplication.Application.Dtos.Profile
{
    public class StudentProfileDto
    {
        public Guid UserId { get; set; }
        public string CurrentLevel { get; set; }
        public string LearningGoals { get; set; }
    }
}
