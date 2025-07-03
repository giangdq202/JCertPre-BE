using JCertPreApplication.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace JCertPreApplication.Application.Dtos.StudyPlan
{
    public class UpdateStudyPlanItemDto
    {
        [MinLength(3, ErrorMessage = "Item name must be at least 3 characters")]
        public string? ItemName { get; set; }

        public string? Description { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public ItemStatus? Status { get; set; }

        public int? OrderIndex { get; set; }
    }
} 