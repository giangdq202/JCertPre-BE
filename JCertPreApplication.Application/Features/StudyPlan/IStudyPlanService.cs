using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCertPreApplication.Application.Features.StudyPlan
{
    public interface IStudyPlanService
    {
        Task<Domain.Entities.StudyPlan> CreateStudyPlanAsync(Domain.Entities.StudyPlan studyPlan);
        Task<Domain.Entities.StudyPlan> GetStudyPlanByIdAsync(Guid planId);
        Task<IEnumerable<Domain.Entities.StudyPlan>> GetAllStudyPlansAsync();
        Task<IEnumerable<Domain.Entities.StudyPlan>> GetStudyPlansByStudentIdAsync(Guid studentId);
        Task<   Domain.Entities.StudyPlan> UpdateStudyPlanAsync(Guid planId, Domain.Entities.StudyPlan studyPlan);
    }
}
