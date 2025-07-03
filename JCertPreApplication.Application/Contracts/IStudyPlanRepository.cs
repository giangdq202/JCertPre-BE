using JCertPreApplication.Domain.Entities;

namespace JCertPreApplication.Application.Contracts
{
    public interface IStudyPlanRepository : IGenericRepository<StudyPlan>
    {
        Task<StudyPlan> CreateStudyPlanAsync(StudyPlan studyPlan);
        Task<StudyPlan> GetStudyPlanByIdAsync(Guid planId);
        Task<IEnumerable<StudyPlan>> GetAllStudyPlansAsync();
        Task<IEnumerable<StudyPlan>> GetStudyPlansByStudentIdAsync(Guid studentId);
        Task<StudyPlan> UpdateStudyPlanAsync(StudyPlan studyPlan);
    }
}
