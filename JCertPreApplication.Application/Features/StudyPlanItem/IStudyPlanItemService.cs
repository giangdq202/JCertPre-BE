using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCertPreApplication.Application.Features.StudyPlanItem
{
    public interface IStudyPlanItemService
    {
        Task<Domain.Entities.StudyPlanItem> CreateStudyPlanItemAsync(Domain.Entities.StudyPlanItem studyPlanItem);
        Task<Domain.Entities.StudyPlanItem> GetStudyPlanItemByIdAsync(Guid itemId);
        Task<IEnumerable<Domain.Entities.StudyPlanItem>> GetStudyPlanItemsByPlanIdAsync(Guid planId);
        Task<Domain.Entities.StudyPlanItem> UpdateStudyPlanItemAsync(Guid itemId, Domain.Entities.StudyPlanItem studyPlanItem);
        Task<bool> DeleteStudyPlanItemAsync(Guid itemId);
    }
}
