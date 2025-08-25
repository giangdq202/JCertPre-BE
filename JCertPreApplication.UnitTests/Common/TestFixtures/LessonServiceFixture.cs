using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Features.Lessons;
using JCertPreApplication.Application.Dtos.Lesson;
using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.UnitTests.Common.Builders;
using Moq;

namespace JCertPreApplication.UnitTests.Common.TestFixtures;

public class LessonServiceFixture
{
    public LessonService LessonService { get; }
    public Mock<ILessonRepository> MockLessonRepository { get; }
    public Mock<ICourseRepository> MockCourseRepository { get; }

    public LessonServiceFixture()
    {
        MockLessonRepository = new Mock<ILessonRepository>();
        MockCourseRepository = new Mock<ICourseRepository>();

        LessonService = new LessonService(
            MockLessonRepository.Object,
            MockCourseRepository.Object
        );
    }

    // Test Data Methods
    public static CreateLessonDto ValidCreateDto() => new()
    {
        Title = "Valid Lesson Title",
        Content = "Valid lesson content for testing purposes",
        LessonOrder = 1
    };

    public static CreateLessonDto CreateDtoWithOrder(int order) => new()
    {
        Title = $"Lesson at Order {order}",
        Content = $"Content for lesson at order {order}",
        LessonOrder = order
    };

    public static UpdateLessonDto ValidUpdateDto() => new()
    {
        Title = "Updated Lesson Title",
        Content = "Updated lesson content for testing purposes",
        LessonOrder = 2
    };

    public static UpdateLessonDto UpdateDtoWithOrder(int order) => new()
    {
        Title = $"Updated Lesson at Order {order}",
        Content = $"Updated content for lesson at order {order}",
        LessonOrder = order
    };

    public static UpdateLessonDto UpdateDtoTitleOnly() => new()
    {
        Title = "Updated Title Only",
        Content = null,
        LessonOrder = null
    };

    public static UpdateLessonDto UpdateDtoContentOnly() => new()
    {
        Title = null,
        Content = "Updated Content Only",
        LessonOrder = null
    };

    public static List<Lesson> CreateLessonList(Guid courseId, int count = 5)
    {
        var lessons = new List<Lesson>();
        for (int i = 1; i <= count; i++)
        {
            lessons.Add(LessonBuilder.Create()
                .WithCourseId(courseId)
                .WithTitle($"Lesson {i}")
                .WithOrder(i)
                .Build());
        }
        return lessons;
    }

    public static List<Lesson> CreateLessonListWithSearch(Guid courseId, string searchTerm, int totalCount = 10)
    {
        var lessons = new List<Lesson>();
        var matchingCount = Math.Min(3, totalCount);

        // Tạo lessons matching search term
        for (int i = 1; i <= matchingCount; i++)
        {
            lessons.Add(LessonBuilder.Create()
                .WithCourseId(courseId)
                .WithTitle($"{searchTerm} Lesson {i}")
                .WithOrder(i)
                .Build());
        }

        // Tạo lessons không matching
        for (int i = matchingCount + 1; i <= totalCount; i++)
        {
            lessons.Add(LessonBuilder.Create()
                .WithCourseId(courseId)
                .WithTitle($"Other Lesson {i}")
                .WithOrder(i)
                .Build());
        }

        return lessons.Where(l => l.title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    public static Pagination<Lesson> CreatePaginatedLessons(List<Lesson> allLessons, int pageIndex, int pageSize, int totalCount)
    {
        var pagedLessons = allLessons.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        
        return new Pagination<Lesson>
        {
            PageIndex = pageIndex - 1, // PageIndex starts from 0 in the actual class
            PageSize = pageSize,
            TotalItemsCount = totalCount,
            Items = pagedLessons
        };
    }

    public static Lesson CreateLessonWithCourse(JCertPreApplication.Domain.Entities.Course course, int order = 1)
    {
        return LessonBuilder.Create()
            .WithCourseId(course.courseId)
            .WithTitle($"Lesson for {course.title}")
            .WithOrder(order)
            .Build();
    }

    public static JCertPreApplication.Domain.Entities.Course CreateTestCourse()
    {
        return CourseBuilder.Create()
            .WithTitle("Test Course for Lessons")
            .Build();
    }

    public static List<Lesson> CreateLessonsForReorder(Guid courseId, int count)
    {
        var lessons = new List<Lesson>();
        for (int i = 1; i <= count; i++)
        {
            lessons.Add(LessonBuilder.Create()
                .WithCourseId(courseId)
                .WithTitle($"Lesson {i}")
                .WithOrder(i)
                .Build());
        }
        return lessons;
    }
}
