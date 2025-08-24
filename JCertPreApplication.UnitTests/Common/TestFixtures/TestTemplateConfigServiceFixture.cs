using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.TestTemplateConfig;
using JCertPreApplication.Application.Features.TestTemplateConfigs;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;
using JCertPreApplication.UnitTests.Common.Builders;
using Moq;
using System.Linq.Expressions;

namespace JCertPreApplication.UnitTests.Common.TestFixtures
{
    public class TestTemplateConfigServiceFixture
    {
        public TestTemplateConfigService TestTemplateConfigService { get; }
        public Mock<ITestTemplateConfigRepository> MockTestTemplateConfigRepository { get; }
        public Mock<ITestTemplateRepository> MockTestTemplateRepository { get; }
        public Mock<ITestTemplateTypeRepository> MockTestTemplateTypeRepository { get; }
        public Mock<IQuestionRepository> MockQuestionRepository { get; }

        public TestTemplateConfigServiceFixture()
        {
            MockTestTemplateConfigRepository = new Mock<ITestTemplateConfigRepository>();
            MockTestTemplateRepository = new Mock<ITestTemplateRepository>();
            MockTestTemplateTypeRepository = new Mock<ITestTemplateTypeRepository>();
            MockQuestionRepository = new Mock<IQuestionRepository>();

            TestTemplateConfigService = new TestTemplateConfigService(
                MockTestTemplateConfigRepository.Object,
                MockTestTemplateRepository.Object,
                MockTestTemplateTypeRepository.Object,
                MockQuestionRepository.Object
            );
        }

        public static CreateTestTemplateConfigDto ValidCreateDto(Guid? subContentId = null)
        {
            return new CreateTestTemplateConfigDto
            {
                subContentId = subContentId ?? Guid.NewGuid(),
                questionCount = 10,
                pointPerQuestion = 5,
                totalPoints = 50,
                sequence = 1
            };
        }

        public static UpdateTestTemplateConfigDto ValidUpdateDto()
        {
            return new UpdateTestTemplateConfigDto
            {
                questionCount = 15,
                pointPerQuestion = 6,
                totalPoints = 90,
                sequence = 2
            };
        }

        public static UpdateTestTemplateConfigDto PartialUpdateDto()
        {
            return new UpdateTestTemplateConfigDto
            {
                questionCount = 12,
                pointPerQuestion = null, // Don't update
                totalPoints = 60,
                sequence = null // Don't update
            };
        }

        public static List<TestTemplateConfig> CreateConfigsForTemplate(Guid templateId, int count)
        {
            var configs = new List<TestTemplateConfig>();
            for (int i = 1; i <= count; i++)
            {
                var subContent = SubContentBuilder.Create()
                    .WithSubContentName(SubContentName.Mondai1)
                    .WithLevel(CourseLevel.N5)
                    .WithContentName(ContentName.Grammar)
                    .Build();

                configs.Add(TestTemplateConfigBuilder.Create()
                    .WithTemplateId(templateId)
                    .WithSequence(i)
                    .WithSubContent(subContent)
                    .Build());
            }
            return configs;
        }

        public void SetupInactiveTypeScenario(Guid templateId, Guid typeId)
        {
            var template = TestTemplateBuilder.Create()
                .WithId(templateId)
                .WithTypeId(typeId)
                .Build();

            MockTestTemplateRepository
                .Setup(x => x.GetByIdAsync(templateId))
                .ReturnsAsync(template);

            MockTestTemplateTypeRepository
                .Setup(x => x.AnyAsync(It.IsAny<Expression<Func<TestTemplateType, bool>>>()))
                .ReturnsAsync(false); // Type is inactive
        }

        public void SetupActiveTypeScenario(Guid templateId, Guid typeId)
        {
            var template = TestTemplateBuilder.Create()
                .WithId(templateId)
                .WithTypeId(typeId)
                .Build();

            MockTestTemplateRepository
                .Setup(x => x.GetByIdAsync(templateId))
                .ReturnsAsync(template);

            MockTestTemplateTypeRepository
                .Setup(x => x.AnyAsync(It.IsAny<Expression<Func<TestTemplateType, bool>>>()))
                .ReturnsAsync(true); // Type is active
        }

        public void SetupQuestionAvailability(Guid subContentId, int availableCount)
        {
            MockQuestionRepository
                .Setup(x => x.CountAsync(It.IsAny<Expression<Func<Question, bool>>>()))
                .ReturnsAsync(availableCount);
        }

        public void ResetMocks()
        {
            MockTestTemplateConfigRepository.Reset();
            MockTestTemplateRepository.Reset();
            MockTestTemplateTypeRepository.Reset();
            MockQuestionRepository.Reset();
        }
    }
}
