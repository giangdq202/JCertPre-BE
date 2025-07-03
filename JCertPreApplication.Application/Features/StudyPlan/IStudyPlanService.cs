using JCertPreApplication.Application.Dtos.StudyPlan;

namespace JCertPreApplication.Application.Features.StudyPlan
{
    public interface IStudyPlanService
    {
        Task<StudyPlanDto> GetStudyPlanByIdAsync(Guid planId);
        Task<IEnumerable<StudyPlanDto>> GetAllStudyPlansAsync();
        Task<StudyPlanDto> CreateStudyPlanAsync(StudyPlanDto studyPlanDto);
        Task<StudyPlanDto> UpdateStudyPlanAsync(Guid planId, UpdateStudyPlanDto updateDto);
        Task DeleteStudyPlanAsync(Guid planId);
        Task<IEnumerable<StudyPlanDto>> GetStudyPlansByStudentIdAsync(Guid studentId);
    }
}
