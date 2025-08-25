using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.TestTemplate;
using JCertPreApplication.Application.Features.TestTemplates;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;
using JCertPreApplication.UnitTests.Common.Builders;
using Moq;

namespace JCertPreApplication.UnitTests.Common.TestFixtures
{
    public class TestTemplateServiceFixture
    {
        public TestTemplateService TestTemplateService { get; }
        public Mock<ITestTemplateRepository> MockTestTemplateRepository { get; }
        public Mock<ITestTemplateTypeRepository> MockTestTemplateTypeRepository { get; }

        public TestTemplateServiceFixture()
        {
            MockTestTemplateRepository = new Mock<ITestTemplateRepository>();
            MockTestTemplateTypeRepository = new Mock<ITestTemplateTypeRepository>();
            
            TestTemplateService = new TestTemplateService(
                MockTestTemplateRepository.Object,
                MockTestTemplateTypeRepository.Object
            );
        }

        public static CreateTestTemplateDto ValidCreateDto(Guid? typeId = null)
        {
            return new CreateTestTemplateDto
            {
                TestTemplateTypeId = typeId ?? Guid.NewGuid(),
                templateName = "Test Template",
                durationMinutes = 60,
                totalScore = 100,
                toPassPercentage = 70m,
                sequence = 1
            };
        }

        public static UpdateTestTemplateDto ValidUpdateDto()
        {
            return new UpdateTestTemplateDto
            {
                templateName = "Updated Template",
                durationMinutes = 90,
                totalScore = 150,
                toPassPercentage = 80m,
                sequence = 2
            };
        }

        public static UpdateTestTemplateDto PartialUpdateDto()
        {
            return new UpdateTestTemplateDto
            {
                templateName = "Partially Updated Template",
                durationMinutes = null, // Don't update
                totalScore = 120,
                toPassPercentage = null, // Don't update
                sequence = null // Don't update
            };
        }

        public static List<TestTemplate> CreateTemplatesForType(Guid typeId, int count)
        {
            var templates = new List<TestTemplate>();
            for (int i = 1; i <= count; i++)
            {
                templates.Add(TestTemplateBuilder.Create()
                    .WithTypeId(typeId)
                    .WithName($"Template {i}")
                    .WithSequence(i)
                    .Build());
            }
            return templates;
        }

        public void ResetMocks()
        {
            MockTestTemplateRepository.Reset();
            MockTestTemplateTypeRepository.Reset();
        }
    }
}
