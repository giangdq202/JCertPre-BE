using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.StudyPlan;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.Application.Features.StudyPlanItem
{
    public class StudyPlanItemService : IStudyPlanItemService
    {
        private readonly IStudyPlanItemRepository _studyPlanItemRepository;
        private readonly IStudyPlanRepository _studyPlanRepository; // To validate planId exists

        public StudyPlanItemService(IStudyPlanItemRepository studyPlanItemRepository, IStudyPlanRepository studyPlanRepository)
        {
            _studyPlanItemRepository = studyPlanItemRepository ?? throw new ArgumentNullException(nameof(studyPlanItemRepository));
            _studyPlanRepository = studyPlanRepository ?? throw new ArgumentNullException(nameof(studyPlanRepository));
        }

        public async Task<StudyPlanItemDto> CreateStudyPlanItemAsync(Guid planId, int sequence, string itemType, Guid? courseId, Guid? testId, ItemStatus status)
        {
            var studyPlanExists = await _studyPlanRepository.GetStudyPlanByIdAsync(planId);
            if (studyPlanExists == null)
                throw ApiException.NotFound("StudyPlan", planId);

            var studyPlanItem = new Domain.Entities.StudyPlanItem
            {
                itemId = Guid.NewGuid(),
                planId = planId,
                sequence = sequence,
                itemType = itemType,
                courseId = courseId,
                testId = testId,
                status = status
            };
            await _studyPlanItemRepository.CreateStudyPlanItemAsync(studyPlanItem);
            return MapToStudyPlanItemDto(studyPlanItem);
        }

        public async Task<StudyPlanItemDto> GetStudyPlanItemByIdAsync(Guid itemId)
        {
            var item = await _studyPlanItemRepository.GetStudyPlanItemByIdAsync(itemId);
            if (item == null)
                throw ApiException.NotFound("StudyPlanItem", itemId);

            return MapToStudyPlanItemDto(item);
        }

        public async Task<IEnumerable<StudyPlanItemDto>> GetStudyPlanItemsByPlanIdAsync(Guid planId)
        {
            var studyPlanExists = await _studyPlanRepository.GetStudyPlanByIdAsync(planId);
            if (studyPlanExists == null)
                throw ApiException.NotFound("StudyPlan", planId);

            var items = await _studyPlanItemRepository.GetStudyPlanItemsByPlanIdAsync(planId);
            return items.Select(MapToStudyPlanItemDto).ToList();
        }

        public async Task<StudyPlanItemDto> UpdateStudyPlanItemAsync(Guid itemId, UpdateStudyPlanItemDto updateDto)
        {
            var existingItem = await _studyPlanItemRepository.GetStudyPlanItemByIdAsync(itemId);
            if (existingItem == null)
                throw ApiException.NotFound("StudyPlanItem", itemId);

            if (updateDto.Sequence.HasValue)
                existingItem.sequence = updateDto.Sequence.Value;
            if (!string.IsNullOrWhiteSpace(updateDto.ItemType))
                existingItem.itemType = updateDto.ItemType;
            if (updateDto.CourseId.HasValue)
                existingItem.courseId = updateDto.CourseId;
            if (updateDto.TestId.HasValue)
                existingItem.testId = updateDto.TestId;
            if (updateDto.Status.HasValue)
                existingItem.status = updateDto.Status.Value;

            var updated = await _studyPlanItemRepository.UpdateStudyPlanItemAsync(existingItem);
            return MapToStudyPlanItemDto(updated);
        }

        public async Task<bool> DeleteStudyPlanItemAsync(Guid itemId)
        {
            var existingItem = await _studyPlanItemRepository.GetStudyPlanItemByIdAsync(itemId);
            if (existingItem == null)
                throw ApiException.NotFound("StudyPlanItem", itemId);

            return await _studyPlanItemRepository.DeleteStudyPlanItemAsync(itemId);
        }

        private StudyPlanItemDto MapToStudyPlanItemDto(Domain.Entities.StudyPlanItem studyPlanItem)
        {
            return new StudyPlanItemDto
            {
                ItemId = studyPlanItem.itemId,
                PlanId = studyPlanItem.planId,
                Sequence = studyPlanItem.sequence,
                ItemType = studyPlanItem.itemType,
                CourseId = studyPlanItem.courseId,
                TestId = studyPlanItem.testId,
                Status = studyPlanItem.status
            };
        }
    }
}
