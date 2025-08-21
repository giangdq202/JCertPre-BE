using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.StudyPlan;
using JCertPreApplication.Application.Exceptions;

namespace JCertPreApplication.Application.Features.StudyPlan
{
    public class StudyPlanService : IStudyPlanService
    {
        private readonly IStudyPlanRepository _studyPlanRepository;
        private readonly IUserRepository _userRepository;

        public StudyPlanService(
            IStudyPlanRepository studyPlanRepository,
            IUserRepository userRepository)
        {
            _studyPlanRepository = studyPlanRepository ?? throw new ArgumentNullException(nameof(studyPlanRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task<StudyPlanDto> GetStudyPlanByIdAsync(Guid planId)
        {
            var studyPlan = await _studyPlanRepository.GetByIdAsync(planId);
            if (studyPlan == null)
                throw ApiException.NotFound("StudyPlan", planId);

            return MapToStudyPlanDto(studyPlan);
        }

        public async Task<IEnumerable<StudyPlanDto>> GetAllStudyPlansAsync()
        {
            var studyPlans = await _studyPlanRepository.GetAllAsync();
            return studyPlans.Select(MapToStudyPlanDto);
        }

        public async Task<StudyPlanDto> CreateStudyPlanAsync(StudyPlanDto studyPlanDto)
        {
            // Validate student exists
            var student = await _userRepository.GetByIdAsync(studyPlanDto.StudentId);
            if (student == null)
                throw ApiException.NotFound("Student", studyPlanDto.StudentId);

            var studyPlan = new Domain.Entities.StudyPlan
            {
                planId = Guid.NewGuid(),
                studentId = studyPlanDto.StudentId,
                createdByStaffId = studyPlanDto.CreatedByStaffId,
                planName = studyPlanDto.PlanName,
                description = studyPlanDto.Description,
                startDate = studyPlanDto.StartDate,
                endDate = studyPlanDto.EndDate
            };

            var created = await _studyPlanRepository.InsertAsync(studyPlan);
            await _studyPlanRepository.SaveChangesAsync();

            return MapToStudyPlanDto(created);
        }

        public async Task<StudyPlanDto> UpdateStudyPlanAsync(Guid planId, UpdateStudyPlanDto updateDto)
        {
            var studyPlan = await _studyPlanRepository.GetByIdAsync(planId);
            if (studyPlan == null)
                throw ApiException.NotFound("StudyPlan", planId);

            // If StudentId is being updated, validate the new student exists
            if (updateDto.StudentId.HasValue && updateDto.StudentId.Value != studyPlan.studentId)
            {
                var student = await _userRepository.GetByIdAsync(updateDto.StudentId.Value);
                if (student == null)
                    throw ApiException.NotFound("Student", updateDto.StudentId.Value);
                studyPlan.studentId = updateDto.StudentId.Value;
            }

            // Update only provided fields
            if (updateDto.PlanName != null)
                studyPlan.planName = updateDto.PlanName;
            if (updateDto.Description != null)
                studyPlan.description = updateDto.Description;
            if (updateDto.StartDate.HasValue)
                studyPlan.startDate = updateDto.StartDate.Value;
            if (updateDto.EndDate.HasValue)
                studyPlan.endDate = updateDto.EndDate.Value;

            await _studyPlanRepository.UpdateAsync(studyPlan);
            await _studyPlanRepository.SaveChangesAsync();

            return MapToStudyPlanDto(studyPlan);
        }

        public async Task DeleteStudyPlanAsync(Guid planId)
        {
            var studyPlan = await _studyPlanRepository.GetByIdAsync(planId);
            if (studyPlan == null)
                throw ApiException.NotFound("StudyPlan", planId);

            // Check if study plan has any items before deleting
            if (studyPlan.StudyPlanItems.Any())
                throw ApiException.BadRequest("STUDY_PLAN_HAS_ITEMS", "Cannot delete study plan that has items. Delete the items first.");

            await _studyPlanRepository.DeleteAsync(studyPlan);
            await _studyPlanRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<StudyPlanDto>> GetStudyPlansByStudentIdAsync(Guid studentId)
        {
            // Validate student exists
            var student = await _userRepository.GetByIdAsync(studentId);
            if (student == null)
                throw ApiException.NotFound("Student", studentId);

            var studyPlans = await _studyPlanRepository.GetAllAsync();
            return studyPlans.Where(sp => sp.studentId == studentId).Select(MapToStudyPlanDto);
        }

        private static StudyPlanDto MapToStudyPlanDto(Domain.Entities.StudyPlan studyPlan)
        {
            return new StudyPlanDto
            {
                PlanId = studyPlan.planId,
                StudentId = studyPlan.studentId,
                CreatedByStaffId = studyPlan.createdByStaffId,
                PlanName = studyPlan.planName,
                Description = studyPlan.description,
                StartDate = studyPlan.startDate,
                EndDate = studyPlan.endDate
            };
        }
    }
}
