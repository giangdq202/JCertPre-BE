using JCertPreApplication.Application.Dtos.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
