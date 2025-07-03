using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.Profile;
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
        public async Task<StudentProfileDto> CreateStudentProfileAsync(Guid userId, string currentLevel, string learningGoals)
        {
            var profile = _studentProfileRepository.CreateStudentProfileAsync(userId, currentLevel, learningGoals);
            return MapToStudentProfileDto(profile);
        }

        public Task<bool> DeleteStudentProfileAsync(Guid userId)
        {
            return _studentProfileRepository.DeleteStudentProfileAsync(userId);
        }

        public async Task<StudentProfileDto> GetStudentProfileAsync(Guid userId)
        {
            var profile = _studentProfileRepository.ReadStudentProfileAsync(userId);
            return MapToStudentProfileDto(profile);
        }

        public async Task<StudentProfileDto> UpdateStudentProfileAsync(Guid userId, string currentLevel, string learningGoals)
        {
            var profile = _studentProfileRepository.UpdateStudentProfileAsync(userId, currentLevel, learningGoals);
            return MapToStudentProfileDto(profile);
        }

        private static StudentProfileDto MapToStudentProfileDto(Task<Domain.Entities.StudentProfile> profile)
        {
            return new StudentProfileDto
            {
                UserId = profile.Result.userId,
                CurrentLevel = profile.Result.currentLevel,
                LearningGoals = profile.Result.learningGoals
            };
        }
    }
}
