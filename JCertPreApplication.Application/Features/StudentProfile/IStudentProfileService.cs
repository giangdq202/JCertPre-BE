using JCertPreApplication.Application.Dtos.Profile;

namespace JCertPreApplication.Application.Features.StudentProfile
{
    public interface IStudentProfileService
    {
        Task<StudentProfileDto> CreateStudentProfileAsync(Guid userId, string currentLevel, string learningGoals);
        Task<StudentProfileDto> GetStudentProfileAsync(Guid userId);
        Task<StudentProfileDto> UpdateStudentProfileAsync(Guid userId, string currentLevel, string learningGoals);
        Task<bool> DeleteStudentProfileAsync(Guid userId);
    }
}
