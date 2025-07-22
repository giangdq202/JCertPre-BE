using System;
using System.Collections.Generic;

namespace JCertPreApplication.Domain.Entities
{
    public class ExamPassThreshold
    {
        public Guid ExamPassThresholdId { get; set; } // PK

        public Guid UserId { get; set; } // FK to User
        public string LevelName { get; set; } = null!;
        public int TotalMaxScore { get; set; }
        public int TotalPassingScore { get; set; }
        public int LanguageKnowledgeMin { get; set; }
        public int LanguageKnowledgeMax { get; set; }
        public int ReadingMax { get; set; }
        public int ReadingMin { get; set; }
        public int ListeningMax { get; set; }
        public int ListeningMin { get; set; }
        public string Status { get; set; } = null!;
        public Guid LastUpdatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public virtual User User { get; set; } = null!;
        public virtual ICollection<Test> Tests { get; set; } = new List<Test>();
    }
}