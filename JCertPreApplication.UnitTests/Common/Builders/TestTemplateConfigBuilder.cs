using JCertPreApplication.Domain.Entities;

namespace JCertPreApplication.UnitTests.Common.Builders
{
    public class TestTemplateConfigBuilder
    {
        private TestTemplateConfig _config;

        public TestTemplateConfigBuilder()
        {
            _config = new TestTemplateConfig
            {
                configId = Guid.NewGuid(),
                templateId = Guid.NewGuid(),
                subContentId = Guid.NewGuid(),
                questionCount = 10,
                pointPerQuestion = 5,
                totalPoints = 50,
                sequence = 1
            };
        }

        public static TestTemplateConfigBuilder Create() => new TestTemplateConfigBuilder();

        public TestTemplateConfigBuilder WithId(Guid id)
        {
            _config.configId = id;
            return this;
        }

        public TestTemplateConfigBuilder WithTemplateId(Guid templateId)
        {
            _config.templateId = templateId;
            return this;
        }

        public TestTemplateConfigBuilder WithSubContentId(Guid subContentId)
        {
            _config.subContentId = subContentId;
            return this;
        }

        public TestTemplateConfigBuilder WithQuestionCount(int count)
        {
            _config.questionCount = count;
            return this;
        }

        public TestTemplateConfigBuilder WithPointPerQuestion(int points)
        {
            _config.pointPerQuestion = points;
            return this;
        }

        public TestTemplateConfigBuilder WithTotalPoints(int total)
        {
            _config.totalPoints = total;
            return this;
        }

        public TestTemplateConfigBuilder WithSequence(int sequence)
        {
            _config.sequence = sequence;
            return this;
        }

        public TestTemplateConfigBuilder WithSubContent(SubContent subContent)
        {
            _config.SubContent = subContent;
            return this;
        }

        public TestTemplateConfig Build() => _config;
    }
}
