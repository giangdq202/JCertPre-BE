using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Features.Enrollment;
using JCertPreApplication.Application.Features.Payment;
using Microsoft.Extensions.Logging;
using Moq;

namespace JCertPreApplication.UnitTests.Common.TestFixtures;

public class EnrollmentServiceFixture : IDisposable
{
    public Mock<IEnrollmentRepository> MockEnrollmentRepository { get; }
    public Mock<IUserRepository> MockUserRepository { get; }
    public Mock<ICourseRepository> MockCourseRepository { get; }
    public Mock<IPaymentService> MockPaymentService { get; }
    public Mock<ILogger<EnrollmentService>> MockLogger { get; }
    public EnrollmentService EnrollmentService { get; }

    public EnrollmentServiceFixture()
    {
        MockEnrollmentRepository = new Mock<IEnrollmentRepository>();
        MockUserRepository = new Mock<IUserRepository>();
        MockCourseRepository = new Mock<ICourseRepository>();
        MockPaymentService = new Mock<IPaymentService>();
        MockLogger = new Mock<ILogger<EnrollmentService>>();

        EnrollmentService = new EnrollmentService(
            MockEnrollmentRepository.Object,
            MockUserRepository.Object,
            MockCourseRepository.Object,
            MockPaymentService.Object,
            MockLogger.Object
        );
    }

    public void Dispose()
    {
        // Cleanup if needed
    }
}
