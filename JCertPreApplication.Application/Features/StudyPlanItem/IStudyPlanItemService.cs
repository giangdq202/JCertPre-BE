using JCertPreApplication.Application.Dtos.StudyPlan;
using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.Application.Features.StudyPlanItem
{
    public interface IStudyPlanItemService
    {
        Task<StudyPlanItemDto> CreateStudyPlanItemAsync(Guid planId, int sequence, string itemType, Guid? courseId, Guid? testId, ItemStatus status, string description);
        Task<StudyPlanItemDto> GetStudyPlanItemByIdAsync(Guid itemId);
        Task<IEnumerable<StudyPlanItemDto>> GetStudyPlanItemsByPlanIdAsync(Guid planId);
        Task<StudyPlanItemDto> UpdateStudyPlanItemAsync(Guid itemId, UpdateStudyPlanItemDto updateDto);
        Task<bool> DeleteStudyPlanItemAsync(Guid itemId);
    }
}
