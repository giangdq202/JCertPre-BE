using System.Net;
using FluentAssertions;
using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.Payment;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Application.Features.Enrollment;
using JCertPreApplication.Application.Features.Payment;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;
using JCertPreApplication.UnitTests.Common.Builders;
using JCertPreApplication.UnitTests.Common.TestFixtures;
using Moq;
using Xunit;

namespace JCertPreApplication.UnitTests.Features.Enrollment;

[Collection("EnrollmentServiceCollection")]
public class EnrollmentServiceTests
{
    private readonly EnrollmentServiceFixture _fixture;
    private readonly EnrollmentService _enrollmentService;
    private readonly Mock<IEnrollmentRepository> _mockEnrollmentRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<ICourseRepository> _mockCourseRepository;
    private readonly Mock<IPaymentService> _mockPaymentService;

    public EnrollmentServiceTests(EnrollmentServiceFixture fixture)
    {
        _fixture = fixture;
        _enrollmentService = _fixture.EnrollmentService;
        _mockEnrollmentRepository = _fixture.MockEnrollmentRepository;
        _mockUserRepository = _fixture.MockUserRepository;
        _mockCourseRepository = _fixture.MockCourseRepository;
        _mockPaymentService = _fixture.MockPaymentService;
    }

    #region EnrollUserAsync Tests

    [Fact]
    public async Task EnrollUserAsync_WithValidData_ShouldSucceedAndReturnResponse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var coursePrice = 100m;
        var paymentId = Guid.NewGuid();

        var user = UserBuilder.Create()
            .WithId(userId)
            .WithCredits(200)
            .Build();

        var course = CourseBuilder.Create()
            .WithId(courseId)
            .WithPrice(coursePrice)
            .WithStatus(CourseStatus.Published)
            .WithTitle("Test Course")
            .Build();

        var paymentResult = new PaymentResult
        {
            IsSuccess = true,
            PaymentId = paymentId,
            RemainingCredit = 100,
            Message = "Payment successful"
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(user);
        _mockCourseRepository.Setup(x => x.GetByIdAsync(courseId))
            .ReturnsAsync(course);
        _mockEnrollmentRepository.Setup(x => x.IsUserEnrolledAsync(userId, courseId))
            .ReturnsAsync(false);
        _mockPaymentService.Setup(x => x.HasSufficientCreditAsync(userId, coursePrice))
            .ReturnsAsync(true);
        _mockPaymentService.Setup(x => x.ProcessCreditPaymentAsync(userId, courseId, coursePrice, It.IsAny<string>()))
            .ReturnsAsync(paymentResult);
        _mockEnrollmentRepository.Setup(x => x.InsertAsync(It.IsAny<Domain.Entities.Enrollment>()))
            .ReturnsAsync((Domain.Entities.Enrollment enrollment) => enrollment);
        _mockEnrollmentRepository.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _enrollmentService.EnrollUserAsync(userId, courseId);

        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be(userId);
        result.CourseId.Should().Be(courseId);
        result.CourseTitle.Should().Be("Test Course");
        result.PricePaid.Should().Be(coursePrice);
        result.RemainingCredit.Should().Be(100);
        result.Message.Should().Be("Successfully enrolled in the course");

        _mockEnrollmentRepository.Verify(x => x.InsertAsync(It.IsAny<Domain.Entities.Enrollment>()), Times.AtLeastOnce);
        _mockEnrollmentRepository.Verify(x => x.SaveChangesAsync(), Times.AtLeastOnce);
    }

    [Fact]
    public async Task EnrollUserAsync_WithNonExistentUser_ShouldThrowNotFoundException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var courseId = Guid.NewGuid();

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync((User?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _enrollmentService.EnrollUserAsync(userId, courseId));

        exception.StatusCode.Should().Be(HttpStatusCode.NotFound);
        exception.ErrorCode.Should().Be("RESOURCE_NOT_FOUND");
        exception.Message.Should().Contain("User");

        _mockPaymentService.Verify(x => x.ProcessCreditPaymentAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<decimal>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task EnrollUserAsync_WithNonExistentCourse_ShouldThrowNotFoundException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var courseId = Guid.NewGuid();

        var user = UserBuilder.Create().WithId(userId).Build();

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(user);
        _mockCourseRepository.Setup(x => x.GetByIdAsync(courseId))
            .ReturnsAsync((Domain.Entities.Course?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _enrollmentService.EnrollUserAsync(userId, courseId));

        exception.StatusCode.Should().Be(HttpStatusCode.NotFound);
        exception.ErrorCode.Should().Be("RESOURCE_NOT_FOUND");
        exception.Message.Should().Contain("Course");

        _mockPaymentService.Verify(x => x.ProcessCreditPaymentAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<decimal>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task EnrollUserAsync_WhenUserAlreadyEnrolled_ShouldThrowBadRequestException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var courseId = Guid.NewGuid();

        var user = UserBuilder.Create().WithId(userId).Build();
        var course = CourseBuilder.Create().WithId(courseId).Build();

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(user);
        _mockCourseRepository.Setup(x => x.GetByIdAsync(courseId))
            .ReturnsAsync(course);
        _mockEnrollmentRepository.Setup(x => x.IsUserEnrolledAsync(userId, courseId))
            .ReturnsAsync(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _enrollmentService.EnrollUserAsync(userId, courseId));

        exception.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        exception.ErrorCode.Should().Be("ALREADY_ENROLLED");
        exception.Message.Should().Be("User is already enrolled in this course");

        _mockPaymentService.Verify(x => x.HasSufficientCreditAsync(It.IsAny<Guid>(), It.IsAny<decimal>()), Times.Never);
    }

    [Fact]
    public async Task EnrollUserAsync_WhenCourseNotPublished_ShouldThrowBadRequestException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var courseId = Guid.NewGuid();

        var user = UserBuilder.Create().WithId(userId).Build();
        var course = CourseBuilder.Create()
            .WithId(courseId)
            .WithStatus(CourseStatus.Draft)
            .Build();

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(user);
        _mockCourseRepository.Setup(x => x.GetByIdAsync(courseId))
            .ReturnsAsync(course);
        _mockEnrollmentRepository.Setup(x => x.IsUserEnrolledAsync(userId, courseId))
            .ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _enrollmentService.EnrollUserAsync(userId, courseId));

        exception.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        exception.ErrorCode.Should().Be("COURSE_NOT_AVAILABLE");
        exception.Message.Should().Be("Course is not available for enrollment");
    }

    [Fact]
    public async Task EnrollUserAsync_WithInsufficientCredit_ShouldThrowBadRequestException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var coursePrice = 100m;

        var user = UserBuilder.Create()
            .WithId(userId)
            .WithCredits(50)
            .Build();

        var course = CourseBuilder.Create()
            .WithId(courseId)
            .WithPrice(coursePrice)
            .WithStatus(CourseStatus.Published)
            .Build();

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(user);
        _mockCourseRepository.Setup(x => x.GetByIdAsync(courseId))
            .ReturnsAsync(course);
        _mockEnrollmentRepository.Setup(x => x.IsUserEnrolledAsync(userId, courseId))
            .ReturnsAsync(false);
        _mockPaymentService.Setup(x => x.HasSufficientCreditAsync(userId, coursePrice))
            .ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _enrollmentService.EnrollUserAsync(userId, courseId));

        exception.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        exception.ErrorCode.Should().Be("INSUFFICIENT_CREDIT");
        exception.Message.Should().Contain("Insufficient credit");
        exception.Message.Should().Contain("Required: 100");
        exception.Message.Should().Contain("Available: 50");

        _mockPaymentService.Verify(x => x.ProcessCreditPaymentAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<decimal>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task EnrollUserAsync_WhenPaymentFails_ShouldThrowBadRequestException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var coursePrice = 100m;

        var user = UserBuilder.Create()
            .WithId(userId)
            .WithCredits(200)
            .Build();

        var course = CourseBuilder.Create()
            .WithId(courseId)
            .WithPrice(coursePrice)
            .WithStatus(CourseStatus.Published)
            .Build();

        var paymentResult = new PaymentResult
        {
            IsSuccess = false,
            Message = "Payment processing failed"
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(user);
        _mockCourseRepository.Setup(x => x.GetByIdAsync(courseId))
            .ReturnsAsync(course);
        _mockEnrollmentRepository.Setup(x => x.IsUserEnrolledAsync(userId, courseId))
            .ReturnsAsync(false);
        _mockPaymentService.Setup(x => x.HasSufficientCreditAsync(userId, coursePrice))
            .ReturnsAsync(true);
        _mockPaymentService.Setup(x => x.ProcessCreditPaymentAsync(userId, courseId, coursePrice, It.IsAny<string>()))
            .ReturnsAsync(paymentResult);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _enrollmentService.EnrollUserAsync(userId, courseId));

        exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        exception.ErrorCode.Should().Be("ENROLLMENT_FAILED");
        exception.Message.Should().Be("An error occurred during enrollment process");

        _mockEnrollmentRepository.Verify(x => x.InsertAsync(It.IsAny<Domain.Entities.Enrollment>()), Times.Never);
    }

    [Fact]
    public async Task EnrollUserAsync_WithEmptyUserId_ShouldThrowBadRequestException()
    {
        // Arrange
        var userId = Guid.Empty;
        var courseId = Guid.NewGuid();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _enrollmentService.EnrollUserAsync(userId, courseId));

        exception.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        exception.ErrorCode.Should().Be("INVALID_USER_ID");
        exception.Message.Should().Be("User ID cannot be empty");
    }

    [Fact]
    public async Task EnrollUserAsync_WithEmptyCourseId_ShouldThrowBadRequestException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var courseId = Guid.Empty;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _enrollmentService.EnrollUserAsync(userId, courseId));

        exception.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        exception.ErrorCode.Should().Be("INVALID_COURSE_ID");
        exception.Message.Should().Be("Course ID cannot be empty");
    }

    #endregion

    #region GetUserEnrollmentsAsync Tests

    [Fact]
    public async Task GetUserEnrollmentsAsync_WithValidUserId_ShouldReturnEnrollments()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var enrollmentId1 = Guid.NewGuid();
        var enrollmentId2 = Guid.NewGuid();
        var courseId1 = Guid.NewGuid();
        var courseId2 = Guid.NewGuid();

        var course1 = CourseBuilder.Create().WithId(courseId1).WithTitle("Course 1").Build();
        var course2 = CourseBuilder.Create().WithId(courseId2).WithTitle("Course 2").Build();

        var enrollments = new List<Domain.Entities.Enrollment>
        {
            EnrollmentBuilder.Create()
                .WithId(enrollmentId1)
                .WithUserId(userId)
                .WithCourseId(courseId1)
                .WithPrice(100m)
                .WithCourse(course1)
                .Build(),
            EnrollmentBuilder.Create()
                .WithId(enrollmentId2)
                .WithUserId(userId)
                .WithCourseId(courseId2)
                .WithPrice(150m)
                .WithCourse(course2)
                .Build()
        };

        _mockEnrollmentRepository.Setup(x => x.GetUserEnrollmentsAsync(userId))
            .ReturnsAsync(enrollments);

        // Act
        var result = await _enrollmentService.GetUserEnrollmentsAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);

        var resultList = result.ToList();
        resultList[0].EnrollmentId.Should().Be(enrollmentId1);
        resultList[0].UserId.Should().Be(userId);
        resultList[0].CourseId.Should().Be(courseId1);
        resultList[0].CourseTitle.Should().Be("Course 1");
        resultList[0].PricePaid.Should().Be(100m);

        resultList[1].EnrollmentId.Should().Be(enrollmentId2);
        resultList[1].CourseTitle.Should().Be("Course 2");
        resultList[1].PricePaid.Should().Be(150m);
    }

    [Fact]
    public async Task GetUserEnrollmentsAsync_WithNoEnrollments_ShouldReturnEmptyList()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _mockEnrollmentRepository.Setup(x => x.GetUserEnrollmentsAsync(userId))
            .ReturnsAsync(new List<Domain.Entities.Enrollment>());

        // Act
        var result = await _enrollmentService.GetUserEnrollmentsAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    #endregion

    #region UnenrollUserAsync Tests

    [Fact]
    public async Task UnenrollUserAsync_WithExistingEnrollment_ShouldReturnTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var courseId = Guid.NewGuid();

        var enrollment = EnrollmentBuilder.Create()
            .WithUserId(userId)
            .WithCourseId(courseId)
            .Build();

        _mockEnrollmentRepository.Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Domain.Entities.Enrollment, bool>>>(), It.IsAny<string>()))
            .ReturnsAsync(enrollment);
        _mockEnrollmentRepository.Setup(x => x.DeleteAsync(enrollment))
            .Returns(Task.CompletedTask);
        _mockEnrollmentRepository.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _enrollmentService.UnenrollUserAsync(userId, courseId);

        // Assert
        result.Should().BeTrue();

        _mockEnrollmentRepository.Verify(x => x.DeleteAsync(enrollment), Times.Once);
        _mockEnrollmentRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UnenrollUserAsync_WithNonExistentEnrollment_ShouldReturnFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var courseId = Guid.NewGuid();

        _mockEnrollmentRepository.Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Domain.Entities.Enrollment, bool>>>(), It.IsAny<string>()))
            .ReturnsAsync((Domain.Entities.Enrollment?)null);

        // Act
        var result = await _enrollmentService.UnenrollUserAsync(userId, courseId);

        // Assert
        result.Should().BeFalse();

        _mockEnrollmentRepository.Verify(x => x.DeleteAsync(It.IsAny<Domain.Entities.Enrollment>()), Times.AtLeastOnce);
        _mockEnrollmentRepository.Verify(x => x.SaveChangesAsync(), Times.AtLeastOnce);
    }

    #endregion

    #region IsUserEnrolledAsync Tests

    [Fact]
    public async Task IsUserEnrolledAsync_WhenEnrolled_ShouldReturnTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var courseId = Guid.NewGuid();

        _mockEnrollmentRepository.Setup(x => x.IsUserEnrolledAsync(userId, courseId))
            .ReturnsAsync(true);

        // Act
        var result = await _enrollmentService.IsUserEnrolledAsync(userId, courseId);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsUserEnrolledAsync_WhenNotEnrolled_ShouldReturnFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var courseId = Guid.NewGuid();

        _mockEnrollmentRepository.Setup(x => x.IsUserEnrolledAsync(userId, courseId))
            .ReturnsAsync(false);

        // Act
        var result = await _enrollmentService.IsUserEnrolledAsync(userId, courseId);

        // Assert
        result.Should().BeFalse();
    }

    #endregion
}

// Collection definition for sharing fixture
[CollectionDefinition("EnrollmentServiceCollection")]
public class EnrollmentServiceCollection : ICollectionFixture<EnrollmentServiceFixture> { }
