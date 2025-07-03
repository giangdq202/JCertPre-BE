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
    public class StudyPlanRepository : GenericRepository<StudyPlan>, IStudyPlanRepository
    {
        private readonly JCertPreDatabaseContext _context;

        public StudyPlanRepository(JCertPreDatabaseContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<StudyPlan> CreateStudyPlanAsync(StudyPlan studyPlan)
        {
            await _context.StudyPlans.AddAsync(studyPlan);
            await _context.SaveChangesAsync();
            return studyPlan;
        }

        public async Task<StudyPlan> GetStudyPlanByIdAsync(Guid planId)
        {
            return await _context.StudyPlans
                .Include(sp => sp.Student)
                .Include(sp => sp.Staff)
                .Include(sp => sp.StudyPlanItems)
                    .ThenInclude(spi => spi.Course)
                .Include(sp => sp.StudyPlanItems)
                    .ThenInclude(spi => spi.Test)
                .FirstOrDefaultAsync(sp => sp.planId == planId);
        }

        public async Task<IEnumerable<StudyPlan>> GetAllStudyPlansAsync()
        {
            return await _context.StudyPlans
                .Include(sp => sp.Student)
                .Include(sp => sp.Staff)
                .Include(sp => sp.StudyPlanItems)
                .ToListAsync();
        }

        public async Task<IEnumerable<StudyPlan>> GetStudyPlansByStudentIdAsync(Guid studentId)
        {
            return await _context.StudyPlans
                .Where(sp => sp.studentId == studentId)
                .Include(sp => sp.Student)
                .Include(sp => sp.Staff)
                .Include(sp => sp.StudyPlanItems)
                .ToListAsync();
        }

        public async Task<StudyPlan> UpdateStudyPlanAsync(StudyPlan studyPlan)
        {
            _context.StudyPlans.Update(studyPlan);
            await _context.SaveChangesAsync();
            return studyPlan;
        }

        public async Task<bool> DeleteStudyPlanAsync(Guid planId)
        {
            var studyPlan = await _context.StudyPlans.FindAsync(planId);
            if (studyPlan == null)
            {
                return false;
            }
            _context.StudyPlans.Remove(studyPlan);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
