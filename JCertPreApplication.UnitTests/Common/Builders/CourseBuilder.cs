using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.UnitTests.Common.Builders;

public class CourseBuilder
{
    private Course _course;

    public CourseBuilder()
    {
        _course = new Course
        {
            courseId = Guid.NewGuid(),
            title = "Test Course",
            description = "Test Course Description",
            price = 100.00m,
            level = CourseLevel.N5,
            status = CourseStatus.Published,
            createdAt = DateTime.UtcNow,
            startDate = DateTime.UtcNow,
            endDate = DateTime.UtcNow.AddMonths(3),
            courseType = CourseType.Public,
            Lessons = new List<Lesson>(),
            Enrollments = new List<Enrollment>(),
            CourseInstructors = new List<CourseInstructor>()
        };
    }

    public static CourseBuilder Create() => new CourseBuilder();

    public CourseBuilder WithId(Guid id)
    {
        _course.courseId = id;
        return this;
    }

    public CourseBuilder WithTitle(string title)
    {
        _course.title = title;
        return this;
    }

    public CourseBuilder WithDescription(string description)
    {
        _course.description = description;
        return this;
    }

    public CourseBuilder WithPrice(decimal price)
    {
        _course.price = price;
        return this;
    }

    public CourseBuilder WithLevel(CourseLevel level)
    {
        _course.level = level;
        return this;
    }

    public CourseBuilder AsInactive()
    {
        _course.status = CourseStatus.Archived;
        return this;
    }

    public CourseBuilder WithThumbnail(string thumbnailUrl)
    {
        _course.thumbnailUrl = thumbnailUrl;
        return this;
    }

    public CourseBuilder WithCreatedAt(DateTime createdAt)
    {
        _course.createdAt = createdAt;
        return this;
    }

    public CourseBuilder WithCourseType(CourseType courseType)
    {
        _course.courseType = courseType;
        return this;
    }

    public CourseBuilder WithStatus(CourseStatus status)
    {
        _course.status = status;
        return this;
    }

    public CourseBuilder WithStartDate(DateTime startDate)
    {
        _course.startDate = startDate;
        return this;
    }

    public CourseBuilder WithEndDate(DateTime endDate)
    {
        _course.endDate = endDate;
        return this;
    }

    public CourseBuilder WithEnrollments(List<Enrollment> enrollments)
    {
        _course.Enrollments = enrollments;
        return this;
    }

    public CourseBuilder WithNoEnrollments()
    {
        _course.Enrollments = new List<Enrollment>();
        return this;
    }

    public CourseBuilder WithCourseInstructors(List<CourseInstructor> courseInstructors)
    {
        _course.CourseInstructors = courseInstructors;
        return this;
    }

    public Course Build() => _course;
}
