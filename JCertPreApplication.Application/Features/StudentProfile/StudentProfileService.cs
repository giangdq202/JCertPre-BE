using JCertPreApplication.Application.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCertPreApplication.Application.Features.StudentProfile
{
    public class StudentProfileService : IStudentProfileService
    {
        private readonly IStudentProfileRepository _studentProfileRepository;
        public StudentProfileService(IStudentProfileRepository studentProfileRepository)
        {
            _studentProfileRepository = studentProfileRepository ?? throw new ArgumentNullException(nameof(studentProfileRepository));
        }
        public Task<Domain.Entities.StudentProfile> CreateStudentProfileAsync(Guid userId, string currentLevel, string learningGoals)
        {
            return _studentProfileRepository.CreateStudentProfileAsync(userId, currentLevel, learningGoals);
        }

        public Task<bool> DeleteStudentProfileAsync(Guid userId)
        {
            return _studentProfileRepository.DeleteStudentProfileAsync(userId);
        }

        public Task<Domain.Entities.StudentProfile> GetStudentProfileAsync(Guid userId)
        {
            return _studentProfileRepository.ReadStudentProfileAsync(userId);
        }

        public Task<Domain.Entities.StudentProfile> UpdateStudentProfileAsync(Guid userId, string currentLevel, string learningGoals)
        {
            return _studentProfileRepository.UpdateStudentProfileAsync(userId, currentLevel, learningGoals);
        }
    }
}
