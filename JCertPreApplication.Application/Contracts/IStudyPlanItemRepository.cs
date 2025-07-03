using JCertPreApplication.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCertPreApplication.Application.Contracts
{
    public interface IStudyPlanItemRepository : IGenericRepository<StudyPlanItem>
    {
        Task<StudyPlanItem> CreateStudyPlanItemAsync(StudyPlanItem studyPlanItem);
        Task<StudyPlanItem> GetStudyPlanItemByIdAsync(Guid itemId);
        Task<IEnumerable<StudyPlanItem>> GetStudyPlanItemsByPlanIdAsync(Guid planId);
        Task<StudyPlanItem> UpdateStudyPlanItemAsync(StudyPlanItem studyPlanItem);
        Task<bool> DeleteStudyPlanItemAsync(Guid itemId);
    }
}
