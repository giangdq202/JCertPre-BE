using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCertPreApplication.Persistence.Repositories
{
    public class StudyPlanItemRepository : GenericRepository<StudyPlanItem>, IStudyPlanItemRepository
    {
        private readonly JCertPreDatabaseContext _context;

        public StudyPlanItemRepository(JCertPreDatabaseContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<StudyPlanItem> CreateStudyPlanItemAsync(StudyPlanItem studyPlanItem)
        {
            await _context.StudyPlanItems.AddAsync(studyPlanItem);
            await _context.SaveChangesAsync();
            return studyPlanItem;
        }

        public async Task<StudyPlanItem> GetStudyPlanItemByIdAsync(Guid itemId)
        {
            return await _context.StudyPlanItems
                .Include(spi => spi.StudyPlan)
                .Include(spi => spi.Course)
                .Include(spi => spi.Test)
                .FirstOrDefaultAsync(spi => spi.itemId == itemId);
        }

        public async Task<IEnumerable<StudyPlanItem>> GetStudyPlanItemsByPlanIdAsync(Guid planId)
        {
            return await _context.StudyPlanItems
                .Where(spi => spi.planId == planId)
                .Include(spi => spi.StudyPlan)
                .Include(spi => spi.Course)
                .Include(spi => spi.Test)
                .OrderBy(spi => spi.sequence) // Order by sequence for logical retrieval
                .ToListAsync();
        }

        public async Task<StudyPlanItem> UpdateStudyPlanItemAsync(StudyPlanItem studyPlanItem)
        {
            _context.StudyPlanItems.Update(studyPlanItem);
            await _context.SaveChangesAsync();
            return studyPlanItem;
        }

        public async Task<bool> DeleteStudyPlanItemAsync(Guid itemId)
        {
            var studyPlanItem = await _context.StudyPlanItems.FindAsync(itemId);
            if (studyPlanItem == null)
            {
                return false;
            }
            _context.StudyPlanItems.Remove(studyPlanItem);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
