using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.StudyPlan;
using JCertPreApplication.Application.Features.StudyPlan;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.UnitTests.Common.Builders;
using Moq;

namespace JCertPreApplication.UnitTests.Common.TestFixtures
{
    /// <summary>
    /// Test fixture for StudyPlanService providing mocked dependencies and helper methods
    /// </summary>
    public class StudyPlanServiceFixture
    {
        public StudyPlanService StudyPlanService { get; }
        public Mock<IStudyPlanRepository> MockStudyPlanRepository { get; }
        public Mock<IUserRepository> MockUserRepository { get; }

        public StudyPlanServiceFixture()
        {
            MockStudyPlanRepository = new Mock<IStudyPlanRepository>();
            MockUserRepository = new Mock<IUserRepository>();

            StudyPlanService = new StudyPlanService(
                MockStudyPlanRepository.Object,
                MockUserRepository.Object
            );
        }

        /// <summary>
        /// Creates a valid StudyPlanDto for testing
        /// </summary>
        public static StudyPlanDto ValidStudyPlanDto(Guid? studentId = null, Guid? staffId = null)
        {
            return new StudyPlanDto
            {
                PlanId = Guid.NewGuid(),
                StudentId = studentId ?? Guid.NewGuid(),
                CreatedByStaffId = staffId ?? Guid.NewGuid(),
                PlanName = "Test Study Plan",
                Description = "Test Description",
                StartDate = DateTime.UtcNow.Date,
                EndDate = DateTime.UtcNow.Date.AddDays(30)
            };
        }

        /// <summary>
        /// Creates a study plan DTO with specific dates
        /// </summary>
        public static StudyPlanDto StudyPlanDtoWithDates(DateTime startDate, DateTime endDate)
        {
            return new StudyPlanDto
            {
                PlanId = Guid.NewGuid(),
                StudentId = Guid.NewGuid(),
                CreatedByStaffId = Guid.NewGuid(),
                PlanName = "Date Test Plan",
                Description = "Testing with specific dates",
                StartDate = startDate,
                EndDate = endDate
            };
        }

        /// <summary>
        /// Creates a list of study plan DTOs
        /// </summary>
        public static List<StudyPlanDto> CreateStudyPlanDtoList(int count = 3)
        {
            var dtos = new List<StudyPlanDto>();
            for (int i = 0; i < count; i++)
            {
                dtos.Add(new StudyPlanDto
                {
                    PlanId = Guid.NewGuid(),
                    StudentId = Guid.NewGuid(),
                    CreatedByStaffId = Guid.NewGuid(),
                    PlanName = $"Plan {i + 1}",
                    Description = $"Description {i + 1}",
                    StartDate = DateTime.UtcNow.Date.AddDays(i),
                    EndDate = DateTime.UtcNow.Date.AddDays(30 + i)
                });
            }
            return dtos;
        }

        /// <summary>
        /// Creates study plans for a specific student
        /// </summary>
        public static List<StudyPlan> CreateStudyPlansForStudent(Guid studentId, int count = 2)
        {
            return StudyPlanBuilder.CreateListForStudent(studentId, count);
        }

        /// <summary>
        /// Creates mixed study plans (some for specific student, some for others)
        /// </summary>
        public static List<StudyPlan> CreateMixedStudyPlans(Guid targetStudentId)
        {
            var plans = new List<StudyPlan>();
            
            // Add 2 plans for target student
            plans.AddRange(StudyPlanBuilder.CreateListForStudent(targetStudentId, 2));
            
            // Add 2 plans for other students
            plans.AddRange(StudyPlanBuilder.CreateList(2));
            
            return plans;
        }

        /// <summary>
        /// Resets all mock setups
        /// </summary>
        public void ResetMocks()
        {
            MockStudyPlanRepository.Reset();
            MockUserRepository.Reset();
        }
    }
}
