using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Features.StudentProfile;
using JCertPreApplication.Domain.Entities;
using Moq;

namespace JCertPreApplication.UnitTests.Common.TestFixtures
{
    public class StudentProfileServiceFixture
    {
        public StudentProfileService StudentProfileService { get; }
        public Mock<IStudentProfileRepository> MockRepository { get; }

        public StudentProfileServiceFixture()
        {
            MockRepository = new Mock<IStudentProfileRepository>();
            StudentProfileService = new StudentProfileService(MockRepository.Object);
        }

        public void SetupCreateProfileAsync(StudentProfile profile)
        {
            MockRepository.Setup(x => x.CreateStudentProfileAsync(
                profile.userId, profile.currentLevel, profile.learningGoals))
                .ReturnsAsync(profile);
        }

        public void SetupReadProfileAsync(StudentProfile profile)
        {
            MockRepository.Setup(x => x.ReadStudentProfileAsync(profile.userId))
                .ReturnsAsync(profile);
        }

        public void SetupUpdateProfileAsync(StudentProfile profile)
        {
            MockRepository.Setup(x => x.UpdateStudentProfileAsync(
                profile.userId, profile.currentLevel, profile.learningGoals))
                .ReturnsAsync(profile);
        }

        public void SetupDeleteProfileAsync(Guid userId, bool result)
        {
            MockRepository.Setup(x => x.DeleteStudentProfileAsync(userId))
                .ReturnsAsync(result);
        }
    }
}
