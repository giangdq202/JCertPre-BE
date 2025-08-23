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
            courseType = CourseType.Public
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

    public Course Build() => _course;
}
