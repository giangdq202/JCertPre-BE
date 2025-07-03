using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.StudyPlan;
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

        public async Task<StudyPlanDto> CreateStudyPlanAsync(Guid studentId, Guid createdByStaffId, string planName, string description, DateTime startDate, DateTime endDate)
        {
            var studyPlan = new Domain.Entities.StudyPlan
            {
                planId = Guid.NewGuid(),
                studentId = studentId,
                createdByStaffId = createdByStaffId,
                planName = planName,
                description = description,
                startDate = startDate,
                endDate = endDate
            };
            await _studyPlanRepository.CreateStudyPlanAsync(studyPlan);
            return MapToStudyPlanDto(studyPlan);
        }

        public async Task<StudyPlanDto> GetStudyPlanByIdAsync(Guid planId)
        {
            var plan =  await _studyPlanRepository.GetStudyPlanByIdAsync(planId);
            return plan != null ? MapToStudyPlanDto(plan) : null; // Return null if not found   
        }

        public async Task<IEnumerable<StudyPlanDto>> GetAllStudyPlansAsync()
        {
            var plan = await _studyPlanRepository.GetAllStudyPlansAsync();
            return plan.Select(MapToStudyPlanDto).ToList(); // Convert to DTOs
        }

        public async Task<IEnumerable<StudyPlanDto>> GetStudyPlansByStudentIdAsync(Guid studentId)
        {
            var plan = await _studyPlanRepository.GetStudyPlansByStudentIdAsync(studentId);
            return plan.Select(MapToStudyPlanDto).ToList(); // Convert to DTOs
        }

        public async Task<StudyPlanDto> UpdateStudyPlanAsync(Guid planId, Domain.Entities.StudyPlan studyPlan)
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

            var plan =  await _studyPlanRepository.UpdateStudyPlanAsync(existingStudyPlan);
            return MapToStudyPlanDto(plan);
        }

        private StudyPlanDto MapToStudyPlanDto(Domain.Entities.StudyPlan studyPlan)
        {
            return new StudyPlanDto
            {
                PlanId = studyPlan.planId,
                StudentId = studyPlan.studentId,
                CreatedByStaffId = studyPlan.createdByStaffId,
                PlanName = studyPlan.planName,
                Description = studyPlan.description,
                StartDate = studyPlan.startDate,
                EndDate = studyPlan.endDate
            };
        }
    }
}
