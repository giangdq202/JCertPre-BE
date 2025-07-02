using JCertPreApplication.Application.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCertPreApplication.Application.Features.StudyPlan
{
    public class StudyPlanService : IStudyPlanService
    {
        private readonly IStudyPlanRepository _studyPlanRepository;

        public StudyPlanService(IStudyPlanRepository studyPlanRepository)
        {
            _studyPlanRepository = studyPlanRepository ?? throw new ArgumentNullException(nameof(studyPlanRepository));
        }

        public async Task<Domain.Entities.StudyPlan> CreateStudyPlanAsync(Domain.Entities.StudyPlan studyPlan)
        {
            // Add any business logic/validation before creating
            return await _studyPlanRepository.CreateStudyPlanAsync(studyPlan);
        }

        public async Task<Domain.Entities.StudyPlan> GetStudyPlanByIdAsync(Guid planId)
        {
            return await _studyPlanRepository.GetStudyPlanByIdAsync(planId);
        }

        public async Task<IEnumerable<Domain.Entities.StudyPlan>> GetAllStudyPlansAsync()
        {
            return await _studyPlanRepository.GetAllStudyPlansAsync();
        }

        public async Task<IEnumerable<Domain.Entities.StudyPlan>> GetStudyPlansByStudentIdAsync(Guid studentId)
        {
            return await _studyPlanRepository.GetStudyPlansByStudentIdAsync(studentId);
        }

        public async Task<Domain.Entities.StudyPlan> UpdateStudyPlanAsync(Guid planId, Domain.Entities.StudyPlan studyPlan)
        {
            var existingStudyPlan = await _studyPlanRepository.GetStudyPlanByIdAsync(planId);
            if (existingStudyPlan == null)
            {
                return null; // Or throw a specific exception
            }

            // Update properties
            existingStudyPlan.planName = studyPlan.planName;
            existingStudyPlan.description = studyPlan.description;
            existingStudyPlan.startDate = studyPlan.startDate;
            existingStudyPlan.endDate = studyPlan.endDate;
            existingStudyPlan.studentId = studyPlan.studentId; // Be careful with changing foreign keys

            // Add any business logic/validation before updating

            return await _studyPlanRepository.UpdateStudyPlanAsync(existingStudyPlan);
        }

        
    }
}
