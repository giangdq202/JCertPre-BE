using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.TestTemplateType;
using JCertPreApplication.Application.Dtos.TestTemplateTypes;
using JCertPreApplication.Application.Features.TestTemplateTypes;
using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;
using JCertPreApplication.UnitTests.Common.Builders;
using Moq;
using System.Linq.Expressions;

namespace JCertPreApplication.UnitTests.Common.TestFixtures
{
    public class TestTemplateTypeServiceFixture
    {
        public TestTemplateTypeService TestTemplateTypeService { get; }
        public Mock<ITestTemplateTypeRepository> MockTestTemplateTypeRepository { get; }
        public Mock<ITestRepository> MockTestRepository { get; }
        public Mock<IGenericRepository<TestTemplate>> MockTestTemplateRepository { get; }
        public Mock<IGenericRepository<TestTemplateConfig>> MockTestTemplateConfigRepository { get; }

        public TestTemplateTypeServiceFixture()
        {
            MockTestTemplateTypeRepository = new Mock<ITestTemplateTypeRepository>();
            MockTestRepository = new Mock<ITestRepository>();
            MockTestTemplateRepository = new Mock<IGenericRepository<TestTemplate>>();
            MockTestTemplateConfigRepository = new Mock<IGenericRepository<TestTemplateConfig>>();

            TestTemplateTypeService = new TestTemplateTypeService(
                MockTestTemplateTypeRepository.Object,
                MockTestRepository.Object,
                MockTestTemplateRepository.Object,
                MockTestTemplateConfigRepository.Object
            );
        }

        public static CreateTestTemplateTypeDto ValidCreateDto(Guid? userId = null)
        {
            return new CreateTestTemplateTypeDto
            {
                userId = userId ?? Guid.NewGuid(),
                typeName = "Test Template Type",
                courseLevel = CourseLevel.N5,
                testType = TestType.JLPTAuto,
                description = "Test description",
                totalTestScore = 100,
                totalPassPercentage = 70m
            };
        }

        public static UpdateTestTemplateTypeDto ValidUpdateDto()
        {
            return new UpdateTestTemplateTypeDto
            {
                typeName = "Updated Template Type",
                courseLevel = CourseLevel.N4,
                testType = TestType.CustomManual,
                description = "Updated description",
                isActive = true,
                totalTestScore = 120,
                totalPassPercentage = 75m
            };
        }

        public static UpdateTestTemplateTypeDto PartialUpdateDto()
        {
            return new UpdateTestTemplateTypeDto
            {
                typeName = "Partially Updated",
                courseLevel = null, // Don't update
                testType = null, // Don't update
                description = "Partial update",
                isActive = null, // Don't update
                totalTestScore = null, // Don't update
                totalPassPercentage = null // Don't update
            };
        }

        public void SetupNoDuplicateScenario(TestType testType, CourseLevel courseLevel)
        {
            MockTestTemplateTypeRepository
                .Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<TestTemplateType, bool>>>(), It.IsAny<string?>()))
                .ReturnsAsync((TestTemplateType?)null);
        }

        public void SetupDuplicateScenario(TestType testType, CourseLevel courseLevel)
        {
            var existingType = TestTemplateTypeBuilder.Create()
                .WithTestType(testType)
                .WithCourseLevel(courseLevel)
                .Build();

            MockTestTemplateTypeRepository
                .Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<TestTemplateType, bool>>>(), It.IsAny<string?>()))
                .ReturnsAsync(existingType);
        }

        public void SetupVerifiedTypeWithTemplatesAndConfigs(Guid typeId)
        {
            var verifiedType = TestTemplateTypeBuilder.Create()
                .WithId(typeId)
                .AsVerified()
                .Build();

            MockTestTemplateTypeRepository
                .Setup(x => x.GetByIdAsync(typeId))
                .ReturnsAsync(verifiedType);

            // Has test templates
            MockTestTemplateRepository
                .Setup(x => x.AnyAsync(It.IsAny<Expression<Func<TestTemplate, bool>>>()))
                .ReturnsAsync(true);

            // Setup templates for config checking
            var templates = new List<TestTemplate>
            {
                TestTemplateBuilder.Create().WithId(Guid.NewGuid()).Build()
            };

            MockTestTemplateRepository
                .Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<TestTemplate, bool>>>(), It.IsAny<string?>()))
                .ReturnsAsync(templates);

            // Has test template configs
            MockTestTemplateConfigRepository
                .Setup(x => x.AnyAsync(It.IsAny<Expression<Func<TestTemplateConfig, bool>>>()))
                .ReturnsAsync(true);
        }

        public void SetupUnverifiedType(Guid typeId)
        {
            var unverifiedType = TestTemplateTypeBuilder.Create()
                .WithId(typeId)
                .AsUnverified()
                .Build();

            MockTestTemplateTypeRepository
                .Setup(x => x.GetByIdAsync(typeId))
                .ReturnsAsync(unverifiedType);
        }

        public void SetupTypeWithoutTemplates(Guid typeId)
        {
            var verifiedType = TestTemplateTypeBuilder.Create()
                .WithId(typeId)
                .AsVerified()
                .Build();

            MockTestTemplateTypeRepository
                .Setup(x => x.GetByIdAsync(typeId))
                .ReturnsAsync(verifiedType);

            // No test templates
            MockTestTemplateRepository
                .Setup(x => x.AnyAsync(It.IsAny<Expression<Func<TestTemplate, bool>>>()))
                .ReturnsAsync(false);
        }

        public void SetupTypeWithoutConfigs(Guid typeId)
        {
            var verifiedType = TestTemplateTypeBuilder.Create()
                .WithId(typeId)
                .AsVerified()
                .Build();

            MockTestTemplateTypeRepository
                .Setup(x => x.GetByIdAsync(typeId))
                .ReturnsAsync(verifiedType);

            // Has test templates
            MockTestTemplateRepository
                .Setup(x => x.AnyAsync(It.IsAny<Expression<Func<TestTemplate, bool>>>()))
                .ReturnsAsync(true);

            var templates = new List<TestTemplate>
            {
                TestTemplateBuilder.Create().WithId(Guid.NewGuid()).Build()
            };

            MockTestTemplateRepository
                .Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<TestTemplate, bool>>>(), It.IsAny<string?>()))
                .ReturnsAsync(templates);

            // No test template configs
            MockTestTemplateConfigRepository
                .Setup(x => x.AnyAsync(It.IsAny<Expression<Func<TestTemplateConfig, bool>>>()))
                .ReturnsAsync(false);
        }

        public void SetupActiveTypeWithOpenTests(Guid typeId)
        {
            var activeType = TestTemplateTypeBuilder.Create()
                .WithId(typeId)
                .AsActive()
                .Build();

            MockTestTemplateTypeRepository
                .Setup(x => x.GetByIdAsync(typeId))
                .ReturnsAsync(activeType);

            // Has open tests
            MockTestRepository
                .Setup(x => x.AnyAsync(It.IsAny<Expression<Func<Test, bool>>>()))
                .ReturnsAsync(true);
        }

        public void SetupPaginationResults(int totalCount, List<TestTemplateType> items)
        {
            var paginationResult = new Pagination<TestTemplateType>
            {
                PageIndex = 1,
                PageSize = 10,
                TotalItemsCount = totalCount,
                Items = items
            };

            MockTestTemplateTypeRepository
                .Setup(x => x.GetPaginationAsync(
                    It.IsAny<Expression<Func<TestTemplateType, bool>>>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<Func<IQueryable<TestTemplateType>, IOrderedQueryable<TestTemplateType>>>()
                ))
                .ReturnsAsync(paginationResult);
        }

        public void ResetMocks()
        {
            MockTestTemplateTypeRepository.Reset();
            MockTestRepository.Reset();
            MockTestTemplateRepository.Reset();
            MockTestTemplateConfigRepository.Reset();
        }
    }
}
