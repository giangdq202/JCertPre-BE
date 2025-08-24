using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.UnitTests.Common.Builders;

public class LessonBuilder
{
    private Lesson _lesson;

    public LessonBuilder()
    {
        _lesson = new Lesson
        {
            lessonId = Guid.NewGuid(),
            courseId = Guid.NewGuid(),
            title = "Sample Lesson",
            lessonOrder = 1,
            content = "Sample lesson content",
            comment = "Sample comment"
        };
    }

    public static LessonBuilder Create() => new LessonBuilder();

    public LessonBuilder WithId(Guid lessonId)
    {
        _lesson.lessonId = lessonId;
        return this;
    }

    public LessonBuilder WithCourseId(Guid courseId)
    {
        _lesson.courseId = courseId;
        return this;
    }

    public LessonBuilder WithTitle(string title)
    {
        _lesson.title = title;
        return this;
    }

    public LessonBuilder WithOrder(int order)
    {
        _lesson.lessonOrder = order;
        return this;
    }

    public Lesson Build() => _lesson;
}
