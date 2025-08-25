using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.Livestream;
using JCertPreApplication.Application.Features.Livestreams;
using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;
using JCertPreApplication.UnitTests.Common.Builders;
using Moq;

namespace JCertPreApplication.UnitTests.Common.TestFixtures
{
    public class LivestreamServiceFixture
    {
        public LivestreamService LivestreamService { get; }
        public Mock<ILivestreamRepository> MockLivestreamRepository { get; }
        public Mock<ICourseRepository> MockCourseRepository { get; }
        public Mock<IEnrollmentRepository> MockEnrollmentRepository { get; }
        public Mock<ICourseInstructorRepository> MockCourseInstructorRepository { get; }
        public Mock<IUserRepository> MockUserRepository { get; }
        public Mock<ILiveKitService> MockLiveKitService { get; }

        public LivestreamServiceFixture()
        {
            MockLivestreamRepository = new Mock<ILivestreamRepository>();
            MockCourseRepository = new Mock<ICourseRepository>();
            MockEnrollmentRepository = new Mock<IEnrollmentRepository>();
            MockCourseInstructorRepository = new Mock<ICourseInstructorRepository>();
            MockUserRepository = new Mock<IUserRepository>();
            MockLiveKitService = new Mock<ILiveKitService>();

            LivestreamService = new LivestreamService(
                MockLivestreamRepository.Object,
                MockCourseRepository.Object,
                MockEnrollmentRepository.Object,
                MockCourseInstructorRepository.Object,
                MockUserRepository.Object,
                MockLiveKitService.Object
            );
        }

        public static CreateLivestreamDto ValidCreateDto(Guid? courseId = null)
        {
            return new CreateLivestreamDto
            {
                CourseId = courseId ?? Guid.NewGuid(),
                Description = "Test livestream description",
                ScheduledDateTime = DateTime.UtcNow.AddHours(2),
                DurationMinutes = 60
            };
        }

        public static UpdateLivestreamDto ValidUpdateDto()
        {
            return new UpdateLivestreamDto
            {
                Description = "Updated livestream description",
                ScheduledDateTime = DateTime.UtcNow.AddHours(3),
                DurationMinutes = 90
            };
        }

        public static UpdateLivestreamDto PartialUpdateDto()
        {
            return new UpdateLivestreamDto
            {
                Description = "Partially updated description",
                ScheduledDateTime = null, // Don't update
                DurationMinutes = null // Don't update
            };
        }

        public void SetupValidCourseScenario(Guid courseId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var course = CourseBuilder.Create()
                .WithId(courseId)
                .WithStartDate(startDate ?? DateTime.UtcNow.Date)
                .WithEndDate(endDate ?? DateTime.UtcNow.Date.AddDays(30))
                .Build();

            MockCourseRepository
                .Setup(x => x.GetByIdAsync(courseId))
                .ReturnsAsync(course);
        }

        public void SetupNonExistentCourseScenario(Guid courseId)
        {
            MockCourseRepository
                .Setup(x => x.GetByIdAsync(courseId))
                .ReturnsAsync((Course?)null);
        }

        public void SetupNoScheduleConflictScenario(Guid courseId, DateTime schedule, int duration, Guid? excludeId = null)
        {
            MockLivestreamRepository
                .Setup(x => x.HasScheduleConflictAsync(courseId, schedule, duration, excludeId))
                .ReturnsAsync(false);
        }

        public void SetupScheduleConflictScenario(Guid courseId, DateTime schedule, int duration, Guid? excludeId = null)
        {
            MockLivestreamRepository
                .Setup(x => x.HasScheduleConflictAsync(courseId, schedule, duration, excludeId))
                .ReturnsAsync(true);
        }

        public void SetupInstructorPermissions(Guid userId, Guid courseId)
        {
            MockCourseInstructorRepository
                .Setup(x => x.IsInstructorAssignedToCourse(courseId, userId))
                .ReturnsAsync(true);

            MockEnrollmentRepository
                .Setup(x => x.IsUserEnrolledAsync(userId, courseId))
                .ReturnsAsync(false);
        }

        public void SetupStudentPermissions(Guid userId, Guid courseId)
        {
            MockCourseInstructorRepository
                .Setup(x => x.IsInstructorAssignedToCourse(courseId, userId))
                .ReturnsAsync(false);

            MockEnrollmentRepository
                .Setup(x => x.IsUserEnrolledAsync(userId, courseId))
                .ReturnsAsync(true);
        }

        public void SetupNoPermissions(Guid userId, Guid courseId)
        {
            MockCourseInstructorRepository
                .Setup(x => x.IsInstructorAssignedToCourse(courseId, userId))
                .ReturnsAsync(false);

            MockEnrollmentRepository
                .Setup(x => x.IsUserEnrolledAsync(userId, courseId))
                .ReturnsAsync(false);
        }

        public void SetupLiveKitToken(string expectedToken)
        {
            MockLiveKitService
                .Setup(x => x.GenerateToken(
                    It.IsAny<string>(), // roomName
                    It.IsAny<string>(), // participantIdentity  
                    It.IsAny<string>(), // participantName
                    It.IsAny<ParticipantRole>(), // role
                    It.IsAny<TimeSpan?>(), // ttl
                    It.IsAny<Dictionary<string, string>?>() // attributes
                ))
                .Returns(expectedToken);
        }

        public void SetupPaginationResults(int totalCount, List<Livestream> items)
        {
            var paginationResult = new Pagination<Livestream>
            {
                PageIndex = 1,
                PageSize = 10,
                TotalItemsCount = totalCount,
                Items = items
            };

            MockLivestreamRepository
                .Setup(x => x.GetLivestreamsWithPaginationAsync(
                    It.IsAny<Guid?>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<int>(),
                    It.IsAny<int>()
                ))
                .ReturnsAsync(paginationResult);
        }

        public void ResetMocks()
        {
            MockLivestreamRepository.Reset();
            MockCourseRepository.Reset();
            MockEnrollmentRepository.Reset();
            MockCourseInstructorRepository.Reset();
            MockUserRepository.Reset();
            MockLiveKitService.Reset();
        }
    }
}
