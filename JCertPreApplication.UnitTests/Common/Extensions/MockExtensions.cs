using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Domain.Entities;
using Moq;

namespace JCertPreApplication.UnitTests.Common.Extensions;

public static class MockExtensions
{
    public static void SetupUserRepository(this Mock<IUserRepository> mock, User user)
    {
        mock.Setup(x => x.GetByIdAsync(user.userId))
            .ReturnsAsync(user);
        mock.Setup(x => x.GetByEmailAsync(user.email))
            .ReturnsAsync(user);
        mock.Setup(x => x.GetByEmailWithRoleAsync(user.email))
            .ReturnsAsync(user);
        mock.Setup(x => x.GetWithRolesAsync(user.userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
    }

    public static void SetupCourseRepository(this Mock<ICourseRepository> mock, Course course)
    {
        mock.Setup(x => x.GetByIdAsync(course.courseId))
            .ReturnsAsync(course);
        mock.Setup(x => x.GetByTitleAsync(course.title))
            .ReturnsAsync(course);
        mock.Setup(x => x.IsTitleUniqueAsync(course.title, null))
            .ReturnsAsync(false);
        mock.Setup(x => x.IsTitleUniqueAsync(course.title, course.courseId))
            .ReturnsAsync(true);
    }

    public static void SetupUserRepositoryNotFound(this Mock<IUserRepository> mock, Guid userId)
    {
        mock.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync((User?)null);
    }

    public static void SetupUserRepositoryNotFound(this Mock<IUserRepository> mock, string email)
    {
        mock.Setup(x => x.GetByEmailAsync(email))
            .ReturnsAsync((User?)null);
        mock.Setup(x => x.GetByEmailWithRoleAsync(email))
            .ReturnsAsync((User?)null);
    }

    public static void SetupCourseRepositoryNotFound(this Mock<ICourseRepository> mock, Guid courseId)
    {
        mock.Setup(x => x.GetByIdAsync(courseId))
            .ReturnsAsync((Course?)null);
    }
}
