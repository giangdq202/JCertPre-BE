using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCertPreApplication.Application.Features.StudentProfile
{
    public interface IStudentProfileService
    {
        Task<Domain.Entities.StudentProfile> CreateStudentProfileAsync(Guid userId, string currentLevel, string learningGoals);
        Task<Domain.Entities.StudentProfile> GetStudentProfileAsync(Guid userId);
        Task<Domain.Entities.StudentProfile> UpdateStudentProfileAsync(Guid userId, string currentLevel, string learningGoals);
        Task<bool> DeleteStudentProfileAsync(Guid userId);
    }
}
