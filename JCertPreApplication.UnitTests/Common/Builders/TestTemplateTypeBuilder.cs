using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.UnitTests.Common.Builders;

public class TestTemplateTypeBuilder
{
    private TestTemplateType _testTemplateType;

    public TestTemplateTypeBuilder()
    {
        _testTemplateType = new TestTemplateType
        {
            TestTemplateTypeId = Guid.NewGuid(),
            userId = Guid.NewGuid(),
            typeName = "Sample Template Type",
            courseLevel = CourseLevel.N5,
            testType = TestType.JLPTAuto,
            description = "Sample description",
            isActive = false,
            createdAt = DateTime.UtcNow,
            totalTestScore = 100,
            totalPassPercentage = 70
        };
    }

    public static TestTemplateTypeBuilder Create() => new TestTemplateTypeBuilder();

    public TestTemplateTypeBuilder WithId(Guid id)
    {
        _testTemplateType.TestTemplateTypeId = id;
        return this;
    }

    public TestTemplateTypeBuilder WithTypeName(string typeName)
    {
        _testTemplateType.typeName = typeName;
        return this;
    }

    public TestTemplateTypeBuilder WithCourseLevel(CourseLevel courseLevel)
    {
        _testTemplateType.courseLevel = courseLevel;
        return this;
    }

    public TestTemplateTypeBuilder WithTestType(TestType testType)
    {
        _testTemplateType.testType = testType;
        return this;
    }

    public TestTemplateTypeBuilder AsActive()
    {
        _testTemplateType.isActive = true;
        return this;
    }

    public TestTemplateTypeBuilder AsInactive()
    {
        _testTemplateType.isActive = false;
        return this;
    }

    public TestTemplateTypeBuilder WithTotalTestScore(int score)
    {
        _testTemplateType.totalTestScore = score;
        return this;
    }

    public TestTemplateTypeBuilder WithTotalPassPercentage(decimal percentage)
    {
        _testTemplateType.totalPassPercentage = percentage;
        return this;
    }

    public TestTemplateTypeBuilder WithUserId(Guid userId)
    {
        _testTemplateType.userId = userId;
        return this;
    }

    public TestTemplateTypeBuilder WithVerifiedUser(Guid verifiedUserId)
    {
        _testTemplateType.verifiedUserId = verifiedUserId;
        return this;
    }

    public TestTemplateTypeBuilder AsVerified()
    {
        _testTemplateType.verifiedUserId = Guid.NewGuid();
        return this;
    }

    public TestTemplateTypeBuilder AsUnverified()
    {
        _testTemplateType.verifiedUserId = null;
        return this;
    }

    public TestTemplateTypeBuilder WithCreatedByUser(User user)
    {
        _testTemplateType.CreatedByUser = user;
        _testTemplateType.userId = user.userId;
        return this;
    }

    public TestTemplateTypeBuilder WithVerifiedByUser(User user)
    {
        _testTemplateType.VerifiedByUser = user;
        _testTemplateType.verifiedUserId = user.userId;
        return this;
    }

    public TestTemplateTypeBuilder WithDescription(string description)
    {
        _testTemplateType.description = description;
        return this;
    }

    public TestTemplateTypeBuilder WithCreatedAt(DateTime createdAt)
    {
        _testTemplateType.createdAt = createdAt;
        return this;
    }

    public TestTemplateType Build() => _testTemplateType;
}
