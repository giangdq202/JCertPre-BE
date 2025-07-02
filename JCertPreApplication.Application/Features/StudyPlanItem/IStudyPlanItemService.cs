using JCertPreApplication.Application.Dtos.StudyPlan;
using JCertPreApplication.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCertPreApplication.Application.Features.StudyPlanItem
{
    public interface IStudyPlanItemService
    {
        Task<StudyPlanItemDto> CreateStudyPlanItemAsync(Guid planId, int sequence, string itemType, Guid? courseId, Guid? testId, ItemStatus status);
        Task<StudyPlanItemDto> GetStudyPlanItemByIdAsync(Guid itemId);
        Task<IEnumerable<StudyPlanItemDto>> GetStudyPlanItemsByPlanIdAsync(Guid planId);
        Task<StudyPlanItemDto> UpdateStudyPlanItemAsync(Guid itemId, Domain.Entities.StudyPlanItem studyPlanItem);
        Task<bool> DeleteStudyPlanItemAsync(Guid itemId);
    }
}
