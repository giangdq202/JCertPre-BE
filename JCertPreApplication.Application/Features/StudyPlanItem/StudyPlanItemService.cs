using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.StudyPlan;
using JCertPreApplication.Application.Exceptions;
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
            try
            {
                // Validate if the associated StudyPlan exists
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
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("STUDY_PLAN_ITEM_CREATE_ERROR", $"An error occurred while creating study plan item: {ex.Message}");
            }
        }

        public async Task<StudyPlanItemDto> GetStudyPlanItemByIdAsync(Guid itemId)
        {
            try
            {
                var item = await _studyPlanItemRepository.GetStudyPlanItemByIdAsync(itemId);
                if (item == null)
                    throw ApiException.NotFound("StudyPlanItem", itemId);

                return MapToStudyPlanItemDto(item);
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("STUDY_PLAN_ITEM_SERVICE_ERROR", $"An error occurred while retrieving study plan item: {ex.Message}");
            }
        }

        public async Task<IEnumerable<StudyPlanItemDto>> GetStudyPlanItemsByPlanIdAsync(Guid planId)
        {
            try
            {
                // Validate if the associated StudyPlan exists
                var studyPlanExists = await _studyPlanRepository.GetStudyPlanByIdAsync(planId);
                if (studyPlanExists == null)
                    throw ApiException.NotFound("StudyPlan", planId);

                var items = await _studyPlanItemRepository.GetStudyPlanItemsByPlanIdAsync(planId);
                return items.Select(MapToStudyPlanItemDto).ToList();
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("STUDY_PLAN_ITEM_SERVICE_ERROR", $"An error occurred while retrieving study plan items: {ex.Message}");
            }
        }

        public async Task<StudyPlanItemDto> UpdateStudyPlanItemAsync(Guid itemId, Domain.Entities.StudyPlanItem studyPlanItem)
        {
            try
            {
                var existingStudyPlanItem = await _studyPlanItemRepository.GetStudyPlanItemByIdAsync(itemId);
                if (existingStudyPlanItem == null)
                    throw ApiException.NotFound("StudyPlanItem", itemId);

                // Ensure the planId is not changed or is valid if changed
                if (existingStudyPlanItem.planId != studyPlanItem.planId)
                {
                    var newStudyPlanExists = await _studyPlanRepository.GetStudyPlanByIdAsync(studyPlanItem.planId);
                    if (newStudyPlanExists == null)
                        throw ApiException.BadRequest("INVALID_STUDY_PLAN", "New associated StudyPlan does not exist.");
                }

                // Update properties
                existingStudyPlanItem.planId = studyPlanItem.planId;
                existingStudyPlanItem.sequence = studyPlanItem.sequence;
                existingStudyPlanItem.itemType = studyPlanItem.itemType;
                existingStudyPlanItem.courseId = studyPlanItem.courseId;
                existingStudyPlanItem.testId = studyPlanItem.testId;
                existingStudyPlanItem.status = studyPlanItem.status;

                // Add any business logic/validation before updating
                var updatedItem = await _studyPlanItemRepository.UpdateStudyPlanItemAsync(existingStudyPlanItem);
                return MapToStudyPlanItemDto(updatedItem);
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("STUDY_PLAN_ITEM_UPDATE_ERROR", $"An error occurred while updating study plan item: {ex.Message}");
            }
        }

        public async Task<bool> DeleteStudyPlanItemAsync(Guid itemId)
        {
            try
            {
                var existingItem = await _studyPlanItemRepository.GetStudyPlanItemByIdAsync(itemId);
                if (existingItem == null)
                    throw ApiException.NotFound("StudyPlanItem", itemId);

                return await _studyPlanItemRepository.DeleteStudyPlanItemAsync(itemId);
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("STUDY_PLAN_ITEM_DELETE_ERROR", $"An error occurred while deleting study plan item: {ex.Message}");
            }
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
