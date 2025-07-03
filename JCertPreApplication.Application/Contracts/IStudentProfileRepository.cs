using JCertPreApplication.Domain.Entities;

namespace JCertPreApplication.Application.Contracts
{
    public interface IStudentProfileRepository : IGenericRepository<StudentProfile>
    {
        Task<StudentProfile> CreateStudentProfileAsync(Guid userId, string currentLevel, string learningGoals);
        Task<StudentProfile> ReadStudentProfileAsync(Guid userId);
        Task<StudentProfile> UpdateStudentProfileAsync(Guid userId, string currentLevel, string learningGoals);
        Task<bool> DeleteStudentProfileAsync(Guid userId);
    }
}
