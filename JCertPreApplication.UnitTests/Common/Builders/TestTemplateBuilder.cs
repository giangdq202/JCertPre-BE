using JCertPreApplication.Domain.Entities;

namespace JCertPreApplication.UnitTests.Common.Builders;

public class TestTemplateBuilder
{
    private TestTemplate _testTemplate;

    public TestTemplateBuilder()
    {
        _testTemplate = new TestTemplate
        {
            templateId = Guid.NewGuid(),
            TestTemplateTypeId = Guid.NewGuid(),
            templateName = "Sample Template",
            durationMinutes = 60,
            totalScore = 100,
            toPassPercentage = 70,
            sequence = 1
        };
    }

    public static TestTemplateBuilder Create() => new TestTemplateBuilder();

    public TestTemplateBuilder WithId(Guid id)
    {
        _testTemplate.templateId = id;
        return this;
    }

    public TestTemplateBuilder WithTestTemplateTypeId(Guid typeId)
    {
        _testTemplate.TestTemplateTypeId = typeId;
        return this;
    }

    public TestTemplateBuilder WithTemplateName(string name)
    {
        _testTemplate.templateName = name;
        return this;
    }

    public TestTemplateBuilder WithDurationMinutes(int duration)
    {
        _testTemplate.durationMinutes = duration;
        return this;
    }

    public TestTemplateBuilder WithTotalScore(int score)
    {
        _testTemplate.totalScore = score;
        return this;
    }

    public TestTemplateBuilder WithToPassPercentage(decimal percentage)
    {
        _testTemplate.toPassPercentage = percentage;
        return this;
    }

    public TestTemplate Build() => _testTemplate;
}
