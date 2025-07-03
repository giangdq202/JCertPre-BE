using JCertPreApplication.Application.Dtos.StudyPlan;

namespace JCertPreApplication.Application.Features.StudyPlan
{
    public interface IStudyPlanService
    {
        Task<StudyPlanDto> CreateStudyPlanAsync(Guid studentId, Guid createdByStaffId, string planName, string description, DateTime startDate, DateTime endDate);
        Task<StudyPlanDto> GetStudyPlanByIdAsync(Guid planId);
        Task<IEnumerable<StudyPlanDto>> GetAllStudyPlansAsync();
        Task<IEnumerable<StudyPlanDto>> GetStudyPlansByStudentIdAsync(Guid studentId);
        Task<StudyPlanDto> UpdateStudyPlanAsync(Guid planId, Domain.Entities.StudyPlan studyPlan);
    }
}
