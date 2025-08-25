using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Features.TestAttempts;
using JCertPreApplication.Persistence.Services.BackgroudServices;
using Microsoft.Extensions.Logging;
using Moq;

namespace JCertPreApplication.UnitTests.Common.TestFixtures;

public class TestAttemptServiceFixture : IDisposable
{
    public TestAttemptService Service { get; }
    public Mock<ITestAttemptRepository> MockTestAttemptRepository { get; }
    public Mock<ITestRepository> MockTestRepository { get; }
    public Mock<IEnrollmentRepository> MockEnrollmentRepository { get; }
    public Mock<IAttemptAnswerRepository> MockAttemptAnswerRepository { get; }
    public Mock<ITestAttemptAutoSubmitController> MockAutoSubmitController { get; }
    public Mock<ILogger<TestAttemptService>> MockLogger { get; }
    public Mock<ITestScoreSummaryRepository> MockTestScoreSummaryRepository { get; }
    public Mock<ILessonRepository> MockLessonRepository { get; }
    public Mock<ITestTemplateRepository> MockTestTemplateRepository { get; }
    public Mock<ITestQuestionRepository> MockTestQuestionRepository { get; }
    public Mock<ITestTemplateTypeRepository> MockTestTemplateTypeRepository { get; }

    public TestAttemptServiceFixture()
    {
        MockTestAttemptRepository = new Mock<ITestAttemptRepository>();
        MockTestRepository = new Mock<ITestRepository>();
        MockEnrollmentRepository = new Mock<IEnrollmentRepository>();
        MockAttemptAnswerRepository = new Mock<IAttemptAnswerRepository>();
        MockAutoSubmitController = new Mock<ITestAttemptAutoSubmitController>();
        MockLogger = new Mock<ILogger<TestAttemptService>>();
        MockTestScoreSummaryRepository = new Mock<ITestScoreSummaryRepository>();
        MockLessonRepository = new Mock<ILessonRepository>();
        MockTestTemplateRepository = new Mock<ITestTemplateRepository>();
        MockTestQuestionRepository = new Mock<ITestQuestionRepository>();
        MockTestTemplateTypeRepository = new Mock<ITestTemplateTypeRepository>();

        Service = new TestAttemptService(
            MockTestAttemptRepository.Object,
            MockTestRepository.Object,
            MockEnrollmentRepository.Object,
            MockAttemptAnswerRepository.Object,
            MockAutoSubmitController.Object,
            MockLogger.Object,
            MockTestScoreSummaryRepository.Object,
            MockLessonRepository.Object,
            MockTestTemplateRepository.Object,
            MockTestQuestionRepository.Object,
            MockTestTemplateTypeRepository.Object
        );
    }

    public void Reset()
    {
        MockTestAttemptRepository.Reset();
        MockTestRepository.Reset();
        MockEnrollmentRepository.Reset();
        MockAttemptAnswerRepository.Reset();
        MockAutoSubmitController.Reset();
        MockLogger.Reset();
        MockTestScoreSummaryRepository.Reset();
        MockLessonRepository.Reset();
        MockTestTemplateRepository.Reset();
        MockTestQuestionRepository.Reset();
        MockTestTemplateTypeRepository.Reset();
    }

    public void Dispose()
    {
        // Cleanup if needed
    }
}
