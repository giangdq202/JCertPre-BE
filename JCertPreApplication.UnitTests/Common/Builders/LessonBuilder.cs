using JCertPreApplication.Domain.Entities;

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
            title = "Test Lesson",
            content = "Test lesson content for unit testing",
            lessonOrder = 1,
            comment = null
        };
    }

    public static LessonBuilder Create() => new LessonBuilder();

    public LessonBuilder WithId(Guid id)
    {
        _lesson.lessonId = id;
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

    public LessonBuilder WithContent(string content)
    {
        _lesson.content = content;
        return this;
    }

    public LessonBuilder WithOrder(int order)
    {
        _lesson.lessonOrder = order;
        return this;
    }

    public LessonBuilder WithComment(string? comment)
    {
        _lesson.comment = comment;
        return this;
    }

    public LessonBuilder WithCourse(Course course)
    {
        _lesson.Course = course;
        _lesson.courseId = course.courseId;
        return this;
    }

    public Lesson Build() => _lesson;
}