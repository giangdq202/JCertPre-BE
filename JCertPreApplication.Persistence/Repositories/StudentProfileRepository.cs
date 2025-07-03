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
    public class StudentProfileRepository : GenericRepository<StudentProfile>, IStudentProfileRepository
    {
        public StudentProfileRepository(JCertPreDatabaseContext context) : base(context)
        {
        }

        public async Task<StudentProfile> CreateStudentProfileAsync(Guid userId, string currentLevel, string learningGoals)
        {
            var user = await _context.Users
             .Include(u => u.Role)
             .FirstOrDefaultAsync(u => u.userId == userId && u.Role.roleName == "Student");
            if (user == null) return null;

            var studentProfile = new StudentProfile
            {
                userId = userId,
                currentLevel = currentLevel,
                learningGoals = learningGoals
            };
            _context.StudentProfiles.Add(studentProfile);
            await _context.SaveChangesAsync();
            return studentProfile;
        }

        public async Task<StudentProfile> ReadStudentProfileAsync(Guid userId)
        {
            var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.userId == userId && u.Role.roleName == "Student");
            if (user == null) return null;

            return await _context.StudentProfiles
                .FirstOrDefaultAsync(sp => sp.userId == userId);
        }

        public async Task<StudentProfile> UpdateStudentProfileAsync(Guid userId, string currentLevel, string learningGoals)
        {
            var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.userId == userId && u.Role.roleName == "Student");
            if (user == null) return null;

            var studentProfile = await _context.StudentProfiles
                .FirstOrDefaultAsync(sp => sp.userId == userId);
            if (studentProfile == null) return null;

            studentProfile.currentLevel = currentLevel;
            studentProfile.learningGoals = learningGoals;
            await _context.SaveChangesAsync();
            return studentProfile;
        }
        public async Task<bool> DeleteStudentProfileAsync(Guid userId)
        {
            var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.userId == userId && u.Role.roleName == "Student");
            if (user == null) return false;

            var studentProfile = await _context.StudentProfiles
                .FirstOrDefaultAsync(sp => sp.userId == userId);
            if (studentProfile == null) return false;

            _context.StudentProfiles.Remove(studentProfile);
            await _context.SaveChangesAsync();
            return true;
        }
    }


}
