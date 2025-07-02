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
    public class InstructorProfileRepository : GenericRepository<InstructorProfile>, IInstructorProfileRepository
    {
        private readonly JCertPreDatabaseContext _context;

        public InstructorProfileRepository(JCertPreDatabaseContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<InstructorProfile> CreateInstructorProfileAsync(Guid userId, string introduction, string? experience, string? teachingStyle)
        {
            var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.userId == userId && u.Role.roleName == "Instructor");
            if (user == null) return null;

            var instructorProfile = new InstructorProfile
            {
                userId = userId,
                introduction = introduction,
                experience = experience,
                teachingStyle = teachingStyle
            };
            _context.InstructorProfiles.Add(instructorProfile);
            await _context.SaveChangesAsync();
            return instructorProfile;
        }

        public async Task<InstructorProfile> ReadInstructorProfileAsync(Guid userId)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.userId == userId && u.Role.roleName == "Instructor");
            if (user == null) return null;

            return await _context.InstructorProfiles
                .FirstOrDefaultAsync(ip => ip.userId == userId);
        }

        public async Task<InstructorProfile> UpdateInstructorProfileAsync(Guid userId, string introduction, string? experience, string? teachingStyle)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.userId == userId && u.Role.roleName == "Instructor");
            if (user == null) return null;

            var instructorProfile = await _context.InstructorProfiles
                .FirstOrDefaultAsync(ip => ip.userId == userId);
            if (instructorProfile == null) return null;

            instructorProfile.introduction = introduction;
            instructorProfile.experience = experience;
            instructorProfile.teachingStyle = teachingStyle;

            _context.InstructorProfiles.Update(instructorProfile);
            await _context.SaveChangesAsync();
            return instructorProfile;
        }

        public async Task<bool> DeleteInstructorProfileAsync(Guid userId)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.userId == userId && u.Role.roleName == "Instructor");
            if (user == null) return false;

            var instructorProfile = await _context.InstructorProfiles
                .FirstOrDefaultAsync(ip => ip.userId == userId);
            if (instructorProfile == null) return false;

            _context.InstructorProfiles.Remove(instructorProfile);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
