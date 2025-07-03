using JCertPreApplication.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
