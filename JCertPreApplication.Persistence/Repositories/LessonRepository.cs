using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.Lesson;
using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace JCertPreApplication.Persistence.Repositories
{
    public class LessonRepository : GenericRepository<Lesson>, ILessonRepository
    {
        public LessonRepository(JCertPreDatabaseContext context) : base(context) { }

        public async Task DeleteAllByCourseIdAsync(Guid courseId)
        {
            var lessons = await _dbSet.Where(l => l.courseId == courseId).ToListAsync();
            _dbSet.RemoveRange(lessons);
        }
    }
}