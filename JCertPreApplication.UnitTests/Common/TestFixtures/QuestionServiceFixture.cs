using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Features.Questions;
using Moq;

namespace JCertPreApplication.UnitTests.Common.TestFixtures
{
    public class QuestionServiceFixture
    {
        public Mock<IQuestionRepository> MockQuestionRepository { get; }
        public Mock<ISubContentRepository> MockSubContentRepository { get; }
        public Mock<IQuestionAttachmentRepository> MockQuestionAttachmentRepository { get; }
        public Mock<IChoiceRepository> MockChoiceRepository { get; }
        public Mock<IFileService> MockFileService { get; }
        public Mock<IAIIntegration> MockAIIntegration { get; }
        public QuestionService Service { get; }

        public QuestionServiceFixture()
        {
            MockQuestionRepository = new Mock<IQuestionRepository>();
            MockSubContentRepository = new Mock<ISubContentRepository>();
            MockQuestionAttachmentRepository = new Mock<IQuestionAttachmentRepository>();
            MockChoiceRepository = new Mock<IChoiceRepository>();
            MockFileService = new Mock<IFileService>();
            MockAIIntegration = new Mock<IAIIntegration>();

            // Initialize service with mocked dependencies
            Service = new QuestionService(
                MockQuestionRepository.Object,
                MockSubContentRepository.Object,
                MockQuestionAttachmentRepository.Object,
                MockChoiceRepository.Object,
                MockFileService.Object,
                MockAIIntegration.Object
            );
        }

        public void Reset()
        {
            MockQuestionRepository.Reset();
            MockSubContentRepository.Reset();
            MockQuestionAttachmentRepository.Reset();
            MockChoiceRepository.Reset();
            MockFileService.Reset();
            MockAIIntegration.Reset();
        }
    }
}
