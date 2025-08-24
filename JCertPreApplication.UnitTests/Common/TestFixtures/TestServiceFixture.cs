using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Features.Tests;
using JCertPreApplication.Application.Features.TestQuestions;
using JCertPreApplication.Domain.Entities;
using Moq;

namespace JCertPreApplication.UnitTests.Common.TestFixtures;

public class TestServiceFixture
{
    public TestService TestService { get; }
    public Mock<ITestRepository> MockTestRepository { get; }
    public Mock<ILessonRepository> MockLessonRepository { get; }
    public Mock<ITestTemplateRepository> MockTestTemplateRepository { get; }
    public Mock<ITestQuestionRepository> MockTestQuestionRepository { get; }
    public Mock<ITestAttemptRepository> MockTestAttemptRepository { get; }
    public Mock<ITestTemplateTypeRepository> MockTestTemplateTypeRepository { get; }
    public Mock<ITestQuestionService> MockTestQuestionService { get; }
    public Mock<IEnrollmentRepository> MockEnrollmentRepository { get; }
    public Mock<IStudentProfileRepository> MockStudentProfileRepository { get; }

    public TestServiceFixture()
    {
        MockTestRepository = new Mock<ITestRepository>();
        MockLessonRepository = new Mock<ILessonRepository>();
        MockTestTemplateRepository = new Mock<ITestTemplateRepository>();
        MockTestQuestionRepository = new Mock<ITestQuestionRepository>();
        MockTestAttemptRepository = new Mock<ITestAttemptRepository>();
        MockTestTemplateTypeRepository = new Mock<ITestTemplateTypeRepository>();
        MockTestQuestionService = new Mock<ITestQuestionService>();
        MockEnrollmentRepository = new Mock<IEnrollmentRepository>();
        MockStudentProfileRepository = new Mock<IStudentProfileRepository>();

        TestService = new TestService(
            MockTestRepository.Object,
            MockLessonRepository.Object,
            MockTestTemplateRepository.Object,
            MockTestQuestionRepository.Object,
            MockTestAttemptRepository.Object,
            MockTestTemplateTypeRepository.Object,
            MockTestQuestionService.Object,
            MockEnrollmentRepository.Object,
            MockStudentProfileRepository.Object
        );
    }
}
