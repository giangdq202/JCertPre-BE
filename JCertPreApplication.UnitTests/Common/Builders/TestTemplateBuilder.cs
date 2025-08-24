using JCertPreApplication.Domain.Entities;

namespace JCertPreApplication.UnitTests.Common.Builders
{
    public class TestTemplateBuilder
    {
        private TestTemplate _testTemplate;

        public TestTemplateBuilder()
        {
            _testTemplate = new TestTemplate
            {
                templateId = Guid.NewGuid(),
                TestTemplateTypeId = Guid.NewGuid(),
                templateName = "Test Template",
                durationMinutes = 60,
                totalScore = 100,
                toPassPercentage = 70m,
                sequence = 1
            };
        }

        public static TestTemplateBuilder Create() => new TestTemplateBuilder();

        public TestTemplateBuilder WithId(Guid id)
        {
            _testTemplate.templateId = id;
            return this;
        }

        public TestTemplateBuilder WithTypeId(Guid typeId)
        {
            _testTemplate.TestTemplateTypeId = typeId;
            return this;
        }

        public TestTemplateBuilder WithTestTemplateTypeId(Guid typeId)
        {
            _testTemplate.TestTemplateTypeId = typeId;
            return this;
        }

        public TestTemplateBuilder WithName(string name)
        {
            _testTemplate.templateName = name;
            return this;
        }

        public TestTemplateBuilder WithDuration(int minutes)
        {
            _testTemplate.durationMinutes = minutes;
            return this;
        }

        public TestTemplateBuilder WithDurationMinutes(int minutes)
        {
            _testTemplate.durationMinutes = minutes;
            return this;
        }

        public TestTemplateBuilder WithScore(int score)
        {
            _testTemplate.totalScore = score;
            return this;
        }

        public TestTemplateBuilder WithPassPercentage(decimal percentage)
        {
            _testTemplate.toPassPercentage = percentage;
            return this;
        }

        public TestTemplateBuilder WithSequence(int sequence)
        {
            _testTemplate.sequence = sequence;
            return this;
        }

        public TestTemplate Build() => _testTemplate;
    }
}