using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.LessonProgress;
using JCertPreApplication.Application.Features.LessonProgresses;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.UnitTests.Common.Builders;
using Moq;

namespace JCertPreApplication.UnitTests.Common.TestFixtures
{
    public class LessonProgressServiceFixture
    {
        public LessonProgressService LessonProgressService { get; }
        public Mock<ILessonProgressRepository> MockLessonProgressRepository { get; }
        public Mock<IEnrollmentRepository> MockEnrollmentRepository { get; }
        public Mock<ILessonRepository> MockLessonRepository { get; }

        public LessonProgressServiceFixture()
        {
            MockLessonProgressRepository = new Mock<ILessonProgressRepository>();
            MockEnrollmentRepository = new Mock<IEnrollmentRepository>();
            MockLessonRepository = new Mock<ILessonRepository>();

            LessonProgressService = new LessonProgressService(
                MockLessonProgressRepository.Object,
                MockEnrollmentRepository.Object,
                MockLessonRepository.Object);
        }

        public void ResetMocks()
        {
            MockLessonProgressRepository.Reset();
            MockEnrollmentRepository.Reset();
            MockLessonRepository.Reset();
        }

        public static CreateLessonProgressDto ValidCreateDto()
        {
            return new CreateLessonProgressDto
            {
                UserId = Guid.NewGuid(),
                LessonId = Guid.NewGuid()
            };
        }

        public static UpdateLessonProgressDto ValidUpdateDto()
        {
            return new UpdateLessonProgressDto
            {
                CompletionRate = 85.5m
            };
        }

        public static Lesson CreateLessonWithOrder(int order, Guid courseId)
        {
            return LessonBuilder.Create()
                .WithCourseId(courseId)
                .WithOrder(order)
                .WithTitle($"Lesson {order}")
                .Build();
        }

        public static Lesson CreateLessonWithTest(Guid testId)
        {
            return LessonBuilder.Create()
                .WithTitle("Lesson with Test")
                .Build();
        }

        public static Test CreateTestForLesson()
        {
            return TestBuilder.Create()
                .WithTitle("Lesson Test")
                .Build();
        }

        public static LessonProgress CreateProgressForUser(Guid userId, int lessonOrder)
        {
            var lessonId = Guid.NewGuid();
            return LessonProgressBuilder.Create()
                .WithUserId(userId)
                .WithLessonId(lessonId)
                .WithCompletionRate(100.0m)
                .Build();
        }

        public static (int LessonOrder, decimal CompletionRate)? CreatePreviousProgress(int order)
        {
            return (order, 100.0m);
        }

        public static List<LessonProgress> CreateUserCourseProgress(Guid userId, Guid courseId, int count)
        {
            var progresses = new List<LessonProgress>();
            for (int i = 1; i <= count; i++)
            {
                var lesson = CreateLessonWithOrder(i, courseId);
                var progress = LessonProgressBuilder.Create()
                    .WithUserId(userId)
                    .WithLessonId(lesson.lessonId)
                    .WithCompletionRate(80.0m + i * 5) // Varying completion rates
                    .WithLesson(lesson)
                    .Build();
                progresses.Add(progress);
            }
            return progresses;
        }

        public static User CreateUserWithFullName(string fullName)
        {
            return UserBuilder.Create()
                .WithName(fullName)
                .Build();
        }

        public static Lesson CreateLessonWithTitle(string title)
        {
            return LessonBuilder.Create()
                .WithTitle(title)
                .Build();
        }
    }
}
