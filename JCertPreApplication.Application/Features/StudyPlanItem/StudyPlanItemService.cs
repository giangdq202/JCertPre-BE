using JCertPreApplication.Application.Contracts;
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

        public async Task<Domain.Entities.StudyPlanItem> CreateStudyPlanItemAsync(Domain.Entities.StudyPlanItem studyPlanItem)
        {
            // Validate if the associated StudyPlan exists
            var studyPlanExists = await _studyPlanRepository.GetStudyPlanByIdAsync(studyPlanItem.planId);
            if (studyPlanExists == null)
            {
                throw new ArgumentException("Associated StudyPlan does not exist.");
            }

            // Add any business logic/validation before creating
            return await _studyPlanItemRepository.CreateStudyPlanItemAsync(studyPlanItem);
        }

        public async Task<Domain.Entities.StudyPlanItem> GetStudyPlanItemByIdAsync(Guid itemId)
        {
            return await _studyPlanItemRepository.GetStudyPlanItemByIdAsync(itemId);
        }

        public async Task<IEnumerable<Domain.Entities.StudyPlanItem>> GetStudyPlanItemsByPlanIdAsync(Guid planId)
        {
            // Validate if the associated StudyPlan exists
            var studyPlanExists = await _studyPlanRepository.GetStudyPlanByIdAsync(planId);
            if (studyPlanExists == null)
            {
                return null; // Or throw a specific exception
            }
            return await _studyPlanItemRepository.GetStudyPlanItemsByPlanIdAsync(planId);
        }

        public async Task<Domain.Entities.StudyPlanItem> UpdateStudyPlanItemAsync(Guid itemId, Domain.Entities.StudyPlanItem studyPlanItem)
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

            return await _studyPlanItemRepository.UpdateStudyPlanItemAsync(existingStudyPlanItem);
        }

        public async Task<bool> DeleteStudyPlanItemAsync(Guid itemId)
        {
            return await _studyPlanItemRepository.DeleteStudyPlanItemAsync(itemId);
        }
    }
}
