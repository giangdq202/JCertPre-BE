using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.StudyPlan;
using JCertPreApplication.Application.Exceptions;

namespace JCertPreApplication.Application.Features.StudyPlan
{
    public class StudyPlanService : IStudyPlanService
    {
        private readonly IStudyPlanRepository _studyPlanRepository;

        public StudyPlanService(IStudyPlanRepository studyPlanRepository)
        {
            _studyPlanRepository = studyPlanRepository ?? throw new ArgumentNullException(nameof(studyPlanRepository));
        }

        public async Task<StudyPlanDto> CreateStudyPlanAsync(Guid studentId, Guid createdByStaffId, string planName, string description, DateTime startDate, DateTime endDate)
        {
            try
            {
                var studyPlan = new Domain.Entities.StudyPlan
                {
                    planId = Guid.NewGuid(),
                    studentId = studentId,
                    createdByStaffId = createdByStaffId,
                    planName = planName,
                    description = description,
                    startDate = startDate,
                    endDate = endDate
                };
                await _studyPlanRepository.CreateStudyPlanAsync(studyPlan);
                return MapToStudyPlanDto(studyPlan);
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("STUDY_PLAN_CREATE_ERROR", $"An error occurred while creating study plan: {ex.Message}");
            }
        }

        public async Task<StudyPlanDto> GetStudyPlanByIdAsync(Guid planId)
        {
            try
            {
                var plan = await _studyPlanRepository.GetStudyPlanByIdAsync(planId);
                if (plan == null)
                    throw ApiException.NotFound("StudyPlan", planId);

                return MapToStudyPlanDto(plan);
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("STUDY_PLAN_SERVICE_ERROR", $"An error occurred while retrieving study plan: {ex.Message}");
            }
        }

        public async Task<IEnumerable<StudyPlanDto>> GetAllStudyPlansAsync()
        {
            try
            {
                var plans = await _studyPlanRepository.GetAllStudyPlansAsync();
                return plans.Select(MapToStudyPlanDto).ToList();
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("STUDY_PLAN_SERVICE_ERROR", $"An error occurred while retrieving study plans: {ex.Message}");
            }
        }

        public async Task<IEnumerable<StudyPlanDto>> GetStudyPlansByStudentIdAsync(Guid studentId)
        {
            try
            {
                var plans = await _studyPlanRepository.GetStudyPlansByStudentIdAsync(studentId);
                return plans.Select(MapToStudyPlanDto).ToList();
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("STUDY_PLAN_SERVICE_ERROR", $"An error occurred while retrieving student study plans: {ex.Message}");
            }
        }

        public async Task<StudyPlanDto> UpdateStudyPlanAsync(Guid planId, Domain.Entities.StudyPlan studyPlan)
        {
            try
            {
                var existingStudyPlan = await _studyPlanRepository.GetStudyPlanByIdAsync(planId);
                if (existingStudyPlan == null)
                    throw ApiException.NotFound("StudyPlan", planId);

                // Update properties
                existingStudyPlan.planName = studyPlan.planName;
                existingStudyPlan.description = studyPlan.description;
                existingStudyPlan.startDate = studyPlan.startDate;
                existingStudyPlan.endDate = studyPlan.endDate;
                existingStudyPlan.studentId = studyPlan.studentId; // Be careful with changing foreign keys

                // Add any business logic/validation before updating
                var updatedPlan = await _studyPlanRepository.UpdateStudyPlanAsync(existingStudyPlan);
                return MapToStudyPlanDto(updatedPlan);
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("STUDY_PLAN_UPDATE_ERROR", $"An error occurred while updating study plan: {ex.Message}");
            }
        }

        private StudyPlanDto MapToStudyPlanDto(Domain.Entities.StudyPlan studyPlan)
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
