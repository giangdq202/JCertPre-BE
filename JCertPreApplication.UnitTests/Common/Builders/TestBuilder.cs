using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.UnitTests.Common.Builders;

public class TestBuilder
{
    private Domain.Entities.Test _test;

    public TestBuilder()
    {
        _test = new Domain.Entities.Test
        {
            testId = Guid.NewGuid(),
            title = "Sample Test",
            description = "Sample test description",
            testType = TestType.CustomManual,
            courseLevel = CourseLevel.N5,
            durationMinutes = 60,
            lessonId = Guid.NewGuid(),
            createdByUserId = Guid.NewGuid(),
            availableFrom = DateTime.UtcNow,
            availableTo = DateTime.UtcNow.AddDays(7),
            maxAttempts = 3,
            passing_percentage = 70,
            status = TestStatus.Close,
            TestTemplateTypeId = null
        };
    }

    public static TestBuilder Create() => new TestBuilder();

    public TestBuilder WithId(Guid testId)
    {
        _test.testId = testId;
        return this;
    }

    public TestBuilder WithTitle(string title)
    {
        _test.title = title;
        return this;
    }

    public TestBuilder WithDescription(string description)
    {
        _test.description = description;
        return this;
    }

    public TestBuilder WithTestType(TestType testType)
    {
        _test.testType = testType;
        return this;
    }

    public TestBuilder WithCourseLevel(CourseLevel courseLevel)
    {
        _test.courseLevel = courseLevel;
        return this;
    }

    public TestBuilder WithDurationMinutes(int durationMinutes)
    {
        _test.durationMinutes = durationMinutes;
        return this;
    }

    public TestBuilder WithLessonId(Guid lessonId)
    {
        _test.lessonId = lessonId;
        return this;
    }

    public TestBuilder WithCreatedByUserId(Guid userId)
    {
        _test.createdByUserId = userId;
        return this;
    }

    public TestBuilder WithAvailableFrom(DateTime availableFrom)
    {
        _test.availableFrom = availableFrom;
        return this;
    }

    public TestBuilder WithAvailableTo(DateTime availableTo)
    {
        _test.availableTo = availableTo;
        return this;
    }

    public TestBuilder WithMaxAttempts(int maxAttempts)
    {
        _test.maxAttempts = maxAttempts;
        return this;
    }

    public TestBuilder WithPassingPercentage(decimal passingPercentage)
    {
        _test.passing_percentage = passingPercentage;
        return this;
    }

    public TestBuilder WithStatus(TestStatus status)
    {
        _test.status = status;
        return this;
    }

    public TestBuilder WithTestTemplateTypeId(Guid? testTemplateTypeId)
    {
        _test.TestTemplateTypeId = testTemplateTypeId;
        return this;
    }

    public TestBuilder WithTestTemplateType(TestTemplateType testTemplateType)
    {
        _test.TestTemplateType = testTemplateType;
        _test.TestTemplateTypeId = testTemplateType.TestTemplateTypeId;
        return this;
    }

    public TestBuilder AsOpen()
    {
        _test.status = TestStatus.Open;
        return this;
    }

    public TestBuilder AsClosed()
    {
        _test.status = TestStatus.Close;
        return this;
    }

    public TestBuilder AsJLPTAuto()
    {
        _test.testType = TestType.JLPTAuto;
        return this;
    }

    public TestBuilder AsEntryAuto()
    {
        _test.testType = TestType.EntryAuto;
        return this;
    }

    public TestBuilder AsCustomManual()
    {
        _test.testType = TestType.CustomManual;
        return this;
    }

    public Domain.Entities.Test Build() => _test;
}
