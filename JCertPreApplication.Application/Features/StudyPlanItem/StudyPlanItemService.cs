using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.StudyPlan;
using JCertPreApplication.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return item != null ? MapToStudyPlanItemDto(item) : null; // Return null if not found
        }

        public async Task<IEnumerable<StudyPlanItemDto>> GetStudyPlanItemsByPlanIdAsync(Guid planId)
        {
            // Validate if the associated StudyPlan exists
            var studyPlanExists = await _studyPlanRepository.GetStudyPlanByIdAsync(planId);
            if (studyPlanExists == null)
            {
                return null; // Or throw a specific exception
            }
            var item =  await _studyPlanItemRepository.GetStudyPlanItemsByPlanIdAsync(planId);
            return item.Select(MapToStudyPlanItemDto).ToList(); // Convert to DTOs
        }

        public async Task<StudyPlanItemDto> UpdateStudyPlanItemAsync(Guid itemId, Domain.Entities.StudyPlanItem studyPlanItem)
        {
            var existingStudyPlanItem = await _studyPlanItemRepository.GetStudyPlanItemByIdAsync(itemId);
            if (existingStudyPlanItem == null)
            {
                return null; // Or throw a specific exception
            }

            // Ensure the planId is not changed or is valid if changed
            if (existingStudyPlanItem.planId != studyPlanItem.planId)
            {
                var newStudyPlanExists = await _studyPlanRepository.GetStudyPlanByIdAsync(studyPlanItem.planId);
                if (newStudyPlanExists == null)
                {
                    throw new ArgumentException("New associated StudyPlan does not exist.");
                }
            }

            // Update properties
            existingStudyPlanItem.planId = studyPlanItem.planId;
            existingStudyPlanItem.sequence = studyPlanItem.sequence;
            existingStudyPlanItem.itemType = studyPlanItem.itemType;
            existingStudyPlanItem.courseId = studyPlanItem.courseId;
            existingStudyPlanItem.testId = studyPlanItem.testId;
            existingStudyPlanItem.status = studyPlanItem.status;

            // Add any business logic/validation before updating

            var item = await _studyPlanItemRepository.UpdateStudyPlanItemAsync(existingStudyPlanItem);
            return MapToStudyPlanItemDto(item);
        }

        public async Task<bool> DeleteStudyPlanItemAsync(Guid itemId)
        {
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
