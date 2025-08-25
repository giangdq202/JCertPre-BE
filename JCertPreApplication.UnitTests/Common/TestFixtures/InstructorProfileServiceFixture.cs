using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Features.InstructorProfile;
using JCertPreApplication.Domain.Entities;
using Moq;

namespace JCertPreApplication.UnitTests.Common.TestFixtures
{
    public class InstructorProfileServiceFixture
    {
        public InstructorProfileService InstructorProfileService { get; }
        public Mock<IInstructorProfileRepository> MockRepository { get; }

        public InstructorProfileServiceFixture()
        {
            MockRepository = new Mock<IInstructorProfileRepository>();
            InstructorProfileService = new InstructorProfileService(MockRepository.Object);
        }

        public void SetupCreateProfileAsync(InstructorProfile profile)
        {
            MockRepository.Setup(x => x.CreateInstructorProfileAsync(
                profile.userId, 
                profile.introduction, 
                profile.experience, 
                profile.teachingStyle))
                .ReturnsAsync(profile);
        }

        public void SetupCreateProfileAsyncReturnsNull(Guid userId, string introduction, string? experience, string? teachingStyle)
        {
            MockRepository.Setup(x => x.CreateInstructorProfileAsync(userId, introduction, experience, teachingStyle))
                .ReturnsAsync((InstructorProfile?)null);
        }

        public void SetupReadProfileAsync(InstructorProfile profile)
        {
            MockRepository.Setup(x => x.ReadInstructorProfileAsync(profile.userId))
                .ReturnsAsync(profile);
        }

        public void SetupReadProfileAsyncReturnsNull(Guid userId)
        {
            MockRepository.Setup(x => x.ReadInstructorProfileAsync(userId))
                .ReturnsAsync((InstructorProfile?)null);
        }

        public void SetupUpdateProfileAsync(InstructorProfile profile)
        {
            MockRepository.Setup(x => x.UpdateInstructorProfileAsync(
                profile.userId, 
                profile.introduction, 
                profile.experience, 
                profile.teachingStyle))
                .ReturnsAsync(profile);
        }

        public void SetupUpdateProfileAsyncReturnsNull(Guid userId, string introduction, string? experience, string? teachingStyle)
        {
            MockRepository.Setup(x => x.UpdateInstructorProfileAsync(userId, introduction, experience, teachingStyle))
                .ReturnsAsync((InstructorProfile?)null);
        }

        public void SetupDeleteProfileAsync(Guid userId, bool result)
        {
            MockRepository.Setup(x => x.DeleteInstructorProfileAsync(userId))
                .ReturnsAsync(result);
        }

        public void SetupRepositoryThrowsException<T>(string methodName, Exception exception) where T : Exception
        {
            switch (methodName)
            {
                case "CreateInstructorProfileAsync":
                    MockRepository.Setup(x => x.CreateInstructorProfileAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                        .ThrowsAsync(exception);
                    break;
                case "ReadInstructorProfileAsync":
                    MockRepository.Setup(x => x.ReadInstructorProfileAsync(It.IsAny<Guid>()))
                        .ThrowsAsync(exception);
                    break;
                case "UpdateInstructorProfileAsync":
                    MockRepository.Setup(x => x.UpdateInstructorProfileAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                        .ThrowsAsync(exception);
                    break;
                case "DeleteInstructorProfileAsync":
                    MockRepository.Setup(x => x.DeleteInstructorProfileAsync(It.IsAny<Guid>()))
                        .ThrowsAsync(exception);
                    break;
            }
        }
    }
}
