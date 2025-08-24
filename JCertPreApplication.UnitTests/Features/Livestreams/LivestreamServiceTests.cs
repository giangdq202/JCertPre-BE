using FluentAssertions;
using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.Livestream;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;
using JCertPreApplication.UnitTests.Common.Builders;
using JCertPreApplication.UnitTests.Common.TestFixtures;
using Moq;
using System.Net;
using Xunit;

namespace JCertPreApplication.UnitTests.Features.Livestreams
{
    public class LivestreamServiceTests : IClassFixture<LivestreamServiceFixture>
    {
        private readonly LivestreamServiceFixture _fixture;

        public LivestreamServiceTests(LivestreamServiceFixture fixture)
        {
            _fixture = fixture;
        }

        #region CreateLivestreamAsync Tests

        [Fact]
        public async Task CreateLivestreamAsync_WithValidData_ShouldCreateLivestream()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var createDto = LivestreamServiceFixture.ValidCreateDto(courseId);

            _fixture.SetupValidCourseScenario(courseId);
            _fixture.SetupNoScheduleConflictScenario(courseId, createDto.ScheduledDateTime, createDto.DurationMinutes);

            var createdLivestream = LivestreamBuilder.Create()
                .WithCourseId(courseId)
                .WithDescription(createDto.Description)
                .WithSchedule(createDto.ScheduledDateTime)
                .WithDuration(createDto.DurationMinutes)
                .AsScheduled()
                .Build();

            _fixture.MockLivestreamRepository
                .Setup(x => x.InsertAsync(It.IsAny<Livestream>()))
                .ReturnsAsync(It.IsAny<Livestream>());

            _fixture.MockLivestreamRepository
                .Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            _fixture.MockLivestreamRepository
                .Setup(x => x.GetLivestreamWithDetailsAsync(It.IsAny<Guid>()))
                .ReturnsAsync(createdLivestream);

            // Act
            var result = await _fixture.LivestreamService.CreateLivestreamAsync(createDto);

            // Assert
            result.Should().NotBeNull();
            result.CourseId.Should().Be(courseId);
            result.Description.Should().Be(createDto.Description);
            result.ScheduledDateTime.Should().Be(createDto.ScheduledDateTime);
            result.DurationMinutes.Should().Be(createDto.DurationMinutes);
            result.Status.Should().Be(LivestreamStatus.SCHEDULED);

            _fixture.MockLivestreamRepository.Verify(x => x.InsertAsync(It.IsAny<Livestream>()), Times.Once);
            _fixture.MockLivestreamRepository.Verify(x => x.SaveChangesAsync(), Times.Once);

            _fixture.ResetMocks();
        }

        [Fact]
        public async Task CreateLivestreamAsync_WithNonExistentCourse_ShouldThrowNotFoundException()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var createDto = LivestreamServiceFixture.ValidCreateDto(courseId);

            _fixture.SetupNonExistentCourseScenario(courseId);

            // Act
            var act = async () => await _fixture.LivestreamService.CreateLivestreamAsync(createDto);

            // Assert
            var exception = await act.Should().ThrowAsync<ApiException>();
            exception.Which.StatusCode.Should().Be(HttpStatusCode.NotFound);
            exception.Which.ErrorCode.Should().Be("RESOURCE_NOT_FOUND");

            _fixture.ResetMocks();
        }

        [Fact]
        public async Task CreateLivestreamAsync_WithPastScheduleTime_ShouldThrowBadRequestException()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var createDto = new CreateLivestreamDto
            {
                CourseId = courseId,
                Description = "Test livestream",
                ScheduledDateTime = DateTime.UtcNow.AddHours(-1), // Past time
                DurationMinutes = 60
            };

            _fixture.SetupValidCourseScenario(courseId);

            // Act
            var act = async () => await _fixture.LivestreamService.CreateLivestreamAsync(createDto);

            // Assert
            var exception = await act.Should().ThrowAsync<ApiException>();
            exception.Which.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            exception.Which.ErrorCode.Should().Be("INVALID_SCHEDULE_TIME");
            exception.Which.Message.Should().Contain("Scheduled time must be in the future");

            _fixture.ResetMocks();
        }

        [Fact]
        public async Task CreateLivestreamAsync_WithScheduleOutsideCourse_ShouldThrowBadRequestException()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var courseStartDate = DateTime.UtcNow.Date.AddDays(10);
            var courseEndDate = DateTime.UtcNow.Date.AddDays(40);

            var createDto = new CreateLivestreamDto
            {
                CourseId = courseId,
                Description = "Test livestream",
                ScheduledDateTime = courseStartDate.AddDays(-1), // Before course starts
                DurationMinutes = 60
            };

            _fixture.SetupValidCourseScenario(courseId, courseStartDate, courseEndDate);

            // Act
            var act = async () => await _fixture.LivestreamService.CreateLivestreamAsync(createDto);

            // Assert
            var exception = await act.Should().ThrowAsync<ApiException>();
            exception.Which.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            exception.Which.ErrorCode.Should().Be("LIVESTREAM_BEFORE_COURSE");
            exception.Which.Message.Should().Contain("cannot be scheduled before course start date");

            _fixture.ResetMocks();
        }

        [Fact]
        public async Task CreateLivestreamAsync_WithScheduleConflict_ShouldThrowBadRequestException()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var createDto = LivestreamServiceFixture.ValidCreateDto(courseId);

            _fixture.SetupValidCourseScenario(courseId);
            _fixture.SetupScheduleConflictScenario(courseId, createDto.ScheduledDateTime, createDto.DurationMinutes);

            // Act
            var act = async () => await _fixture.LivestreamService.CreateLivestreamAsync(createDto);

            // Assert
            var exception = await act.Should().ThrowAsync<ApiException>();
            exception.Which.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            exception.Which.ErrorCode.Should().Be("SCHEDULE_CONFLICT");
            exception.Which.Message.Should().Contain("There is already a livestream scheduled at this time");

            _fixture.ResetMocks();
        }

        #endregion

        #region GetLivestreamByIdAsync Tests

        [Fact]
        public async Task GetLivestreamByIdAsync_WithExistingId_ShouldReturnLivestream()
        {
            // Arrange
            var livestreamId = Guid.NewGuid();
            var courseId = Guid.NewGuid();

            var livestream = LivestreamBuilder.Create()
                .WithId(livestreamId)
                .WithCourseId(courseId)
                .WithDescription("Test livestream")
                .AsScheduled()
                .Build();

            _fixture.MockLivestreamRepository
                .Setup(x => x.GetLivestreamWithDetailsAsync(livestreamId))
                .ReturnsAsync(livestream);

            // Act
            var result = await _fixture.LivestreamService.GetLivestreamByIdAsync(livestreamId);

            // Assert
            result.Should().NotBeNull();
            result!.LivestreamId.Should().Be(livestreamId);
            result.CourseId.Should().Be(courseId);
            result.Description.Should().Be("Test livestream");
            result.Status.Should().Be(LivestreamStatus.SCHEDULED);

            _fixture.ResetMocks();
        }

        [Fact]
        public async Task GetLivestreamByIdAsync_WithNonExistentId_ShouldReturnNull()
        {
            // Arrange
            var livestreamId = Guid.NewGuid();

            _fixture.MockLivestreamRepository
                .Setup(x => x.GetLivestreamWithDetailsAsync(livestreamId))
                .ReturnsAsync((Livestream?)null);

            // Act
            var result = await _fixture.LivestreamService.GetLivestreamByIdAsync(livestreamId);

            // Assert
            result.Should().BeNull();

            _fixture.ResetMocks();
        }

        #endregion

        #region UpdateLivestreamAsync Tests

        [Fact]
        public async Task UpdateLivestreamAsync_WithValidData_ShouldUpdateLivestream()
        {
            // Arrange
            var livestreamId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var updateDto = LivestreamServiceFixture.ValidUpdateDto();

            var existingLivestream = LivestreamBuilder.Create()
                .WithId(livestreamId)
                .WithCourseId(courseId)
                .AsScheduled()
                .Build();

            _fixture.MockLivestreamRepository
                .Setup(x => x.GetByIdAsync(livestreamId))
                .ReturnsAsync(existingLivestream);

            _fixture.SetupValidCourseScenario(courseId);
            _fixture.SetupNoScheduleConflictScenario(courseId, updateDto.ScheduledDateTime!.Value, updateDto.DurationMinutes!.Value, livestreamId);

            var updatedLivestream = LivestreamBuilder.Create()
                .WithId(livestreamId)
                .WithCourseId(courseId)
                .WithDescription(updateDto.Description!)
                .WithSchedule(updateDto.ScheduledDateTime!.Value)
                .WithDuration(updateDto.DurationMinutes!.Value)
                .AsScheduled()
                .Build();

            _fixture.MockLivestreamRepository
                .Setup(x => x.UpdateAsync(It.IsAny<Livestream>()))
                .Returns(Task.CompletedTask);

            _fixture.MockLivestreamRepository
                .Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            _fixture.MockLivestreamRepository
                .Setup(x => x.GetLivestreamWithDetailsAsync(livestreamId))
                .ReturnsAsync(updatedLivestream);

            // Act
            var result = await _fixture.LivestreamService.UpdateLivestreamAsync(livestreamId, updateDto);

            // Assert
            result.Should().NotBeNull();
            result.LivestreamId.Should().Be(livestreamId);
            result.Description.Should().Be(updateDto.Description);
            result.ScheduledDateTime.Should().Be(updateDto.ScheduledDateTime);
            result.DurationMinutes.Should().Be(updateDto.DurationMinutes);

            _fixture.MockLivestreamRepository.Verify(x => x.UpdateAsync(It.IsAny<Livestream>()), Times.Once);
            _fixture.MockLivestreamRepository.Verify(x => x.SaveChangesAsync(), Times.Once);

            _fixture.ResetMocks();
        }

        [Fact]
        public async Task UpdateLivestreamAsync_WithActiveLivestream_ShouldThrowBadRequestException()
        {
            // Arrange
            var livestreamId = Guid.NewGuid();
            var updateDto = LivestreamServiceFixture.ValidUpdateDto();

            var liveLivestream = LivestreamBuilder.Create()
                .WithId(livestreamId)
                .AsLive() // Status = LIVE
                .Build();

            _fixture.MockLivestreamRepository
                .Setup(x => x.GetByIdAsync(livestreamId))
                .ReturnsAsync(liveLivestream);

            // Act
            var act = async () => await _fixture.LivestreamService.UpdateLivestreamAsync(livestreamId, updateDto);

            // Assert
            var exception = await act.Should().ThrowAsync<ApiException>();
            exception.Which.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            exception.Which.ErrorCode.Should().Be("LIVESTREAM_ACTIVE");
            exception.Which.Message.Should().Contain("Cannot update active livestream");

            _fixture.ResetMocks();
        }

        [Fact]
        public async Task UpdateLivestreamAsync_WithCompletedLivestream_ShouldThrowBadRequestException()
        {
            // Arrange
            var livestreamId = Guid.NewGuid();
            var updateDto = LivestreamServiceFixture.ValidUpdateDto();

            var completedLivestream = LivestreamBuilder.Create()
                .WithId(livestreamId)
                .AsCompleted() // Status = COMPLETED
                .Build();

            _fixture.MockLivestreamRepository
                .Setup(x => x.GetByIdAsync(livestreamId))
                .ReturnsAsync(completedLivestream);

            // Act
            var act = async () => await _fixture.LivestreamService.UpdateLivestreamAsync(livestreamId, updateDto);

            // Assert
            var exception = await act.Should().ThrowAsync<ApiException>();
            exception.Which.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            exception.Which.ErrorCode.Should().Be("LIVESTREAM_COMPLETED");
            exception.Which.Message.Should().Contain("Cannot update completed livestream");

            _fixture.ResetMocks();
        }

        [Fact]
        public async Task UpdateLivestreamAsync_WithScheduleConflict_ShouldThrowBadRequestException()
        {
            // Arrange
            var livestreamId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var updateDto = LivestreamServiceFixture.ValidUpdateDto();

            var existingLivestream = LivestreamBuilder.Create()
                .WithId(livestreamId)
                .WithCourseId(courseId)
                .AsScheduled()
                .Build();

            _fixture.MockLivestreamRepository
                .Setup(x => x.GetByIdAsync(livestreamId))
                .ReturnsAsync(existingLivestream);

            _fixture.SetupValidCourseScenario(courseId);
            _fixture.SetupScheduleConflictScenario(courseId, updateDto.ScheduledDateTime!.Value, updateDto.DurationMinutes!.Value, livestreamId);

            // Act
            var act = async () => await _fixture.LivestreamService.UpdateLivestreamAsync(livestreamId, updateDto);

            // Assert
            var exception = await act.Should().ThrowAsync<ApiException>();
            exception.Which.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            exception.Which.ErrorCode.Should().Be("SCHEDULE_CONFLICT");
            exception.Which.Message.Should().Contain("There is already a livestream scheduled at this time");

            _fixture.ResetMocks();
        }

        #endregion

        #region DeleteLivestreamAsync Tests

        [Fact]
        public async Task DeleteLivestreamAsync_WithScheduledLivestream_ShouldDeleteLivestream()
        {
            // Arrange
            var livestreamId = Guid.NewGuid();

            var scheduledLivestream = LivestreamBuilder.Create()
                .WithId(livestreamId)
                .AsScheduled()
                .Build();

            _fixture.MockLivestreamRepository
                .Setup(x => x.GetByIdAsync(livestreamId))
                .ReturnsAsync(scheduledLivestream);

            _fixture.MockLivestreamRepository
                .Setup(x => x.DeleteAsync(It.IsAny<Livestream>()))
                .Returns(Task.CompletedTask);

            _fixture.MockLivestreamRepository
                .Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            await _fixture.LivestreamService.DeleteLivestreamAsync(livestreamId);

            // Assert
            _fixture.MockLivestreamRepository.Verify(x => x.DeleteAsync(scheduledLivestream), Times.Once);
            _fixture.MockLivestreamRepository.Verify(x => x.SaveChangesAsync(), Times.Once);

            _fixture.ResetMocks();
        }

        [Fact]
        public async Task DeleteLivestreamAsync_WithActiveLivestream_ShouldThrowBadRequestException()
        {
            // Arrange
            var livestreamId = Guid.NewGuid();

            var liveLivestream = LivestreamBuilder.Create()
                .WithId(livestreamId)
                .AsLive() // Status = LIVE
                .Build();

            _fixture.MockLivestreamRepository
                .Setup(x => x.GetByIdAsync(livestreamId))
                .ReturnsAsync(liveLivestream);

            // Act
            var act = async () => await _fixture.LivestreamService.DeleteLivestreamAsync(livestreamId);

            // Assert
            var exception = await act.Should().ThrowAsync<ApiException>();
            exception.Which.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            exception.Which.ErrorCode.Should().Be("LIVESTREAM_ACTIVE");
            exception.Which.Message.Should().Contain("Cannot delete active livestream");

            _fixture.ResetMocks();
        }

        #endregion

        #region GetLivestreamsAsync Tests

        [Fact]
        public async Task GetLivestreamsAsync_WithFilters_ShouldReturnPaginatedResults()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var startDate = DateTime.UtcNow.Date;
            var endDate = DateTime.UtcNow.Date.AddDays(7);

            var livestreams = new List<Livestream>
            {
                LivestreamBuilder.Create()
                    .WithCourseId(courseId)
                    .WithSchedule(DateTime.UtcNow.AddDays(1))
                    .AsScheduled()
                    .Build(),
                LivestreamBuilder.Create()
                    .WithCourseId(courseId)
                    .WithSchedule(DateTime.UtcNow.AddDays(2))
                    .AsScheduled()
                    .Build()
            };

            _fixture.SetupPaginationResults(2, livestreams);

            // Act
            var result = await _fixture.LivestreamService.GetLivestreamsAsync(
                courseId, startDate, endDate, 1, 10);

            // Assert
            result.Should().NotBeNull();
            result.PageIndex.Should().Be(1);
            result.PageSize.Should().Be(10);
            result.TotalItemsCount.Should().Be(2);
            result.Items.Should().HaveCount(2);
            result.Items.Should().AllSatisfy(x => x.CourseId.Should().Be(courseId));

            _fixture.ResetMocks();
        }

        [Fact]
        public async Task GetLivestreamsAsync_WithNoFilters_ShouldReturnAllResults()
        {
            // Arrange
            var livestreams = new List<Livestream>
            {
                LivestreamBuilder.Create().AsScheduled().Build(),
                LivestreamBuilder.Create().AsLive().Build(),
                LivestreamBuilder.Create().AsCompleted().Build()
            };

            _fixture.SetupPaginationResults(3, livestreams);

            // Act
            var result = await _fixture.LivestreamService.GetLivestreamsAsync();

            // Assert
            result.Should().NotBeNull();
            result.TotalItemsCount.Should().Be(3);
            result.Items.Should().HaveCount(3);

            _fixture.ResetMocks();
        }

        #endregion

        #region GenerateJoinTokenAsync Tests

        [Fact]
        public async Task GenerateJoinTokenAsync_WithValidInstructor_ShouldGenerateToken()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var livestreamId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var expectedToken = "livekit_instructor_token";

            var user = UserBuilder.Create()
                .WithId(userId)
                .WithName("Test Instructor")
                .Build();

            var livestream = LivestreamBuilder.Create()
                .WithId(livestreamId)
                .WithCourseId(courseId)
                .WithDescription("Test livestream")
                .WithSchedule(DateTime.UtcNow)
                .WithDuration(60)
                .AsLive()
                .Build();

            _fixture.MockLivestreamRepository
                .Setup(x => x.GetLivestreamWithDetailsAsync(livestreamId))
                .ReturnsAsync(livestream);

            _fixture.MockUserRepository
                .Setup(x => x.GetWithRolesAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _fixture.SetupInstructorPermissions(userId, courseId);
            _fixture.SetupLiveKitToken(expectedToken);

            // Act
            var result = await _fixture.LivestreamService.GenerateJoinTokenAsync(userId, livestreamId);

            // Assert
            result.Should().NotBeNull();
            result.Token.Should().Be(expectedToken);
            result.RoomName.Should().Be(livestreamId.ToString());
            result.Description.Should().Be(livestream.description);
            result.ScheduledDateTime.Should().Be(livestream.scheduledDateTime);
            result.DurationMinutes.Should().Be(livestream.durationMinutes);

            _fixture.MockLiveKitService.Verify(x => x.GenerateToken(
                livestreamId.ToString(), // roomName
                userId.ToString(), // participantIdentity
                "Test Instructor", // participantName
                ParticipantRole.Instructor, // role
                It.IsAny<TimeSpan?>(), // ttl
                It.IsAny<Dictionary<string, string>?>() // attributes
            ), Times.Once);

            _fixture.ResetMocks();
        }

        [Fact]
        public async Task GenerateJoinTokenAsync_WithValidStudent_ShouldGenerateTokenWithStudentRole()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var livestreamId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var expectedToken = "livekit_student_token";

            var user = UserBuilder.Create()
                .WithId(userId)
                .WithName("Test Student")
                .Build();

            var livestream = LivestreamBuilder.Create()
                .WithId(livestreamId)
                .WithCourseId(courseId)
                .AsLive()
                .Build();

            _fixture.MockLivestreamRepository
                .Setup(x => x.GetLivestreamWithDetailsAsync(livestreamId))
                .ReturnsAsync(livestream);

            _fixture.MockUserRepository
                .Setup(x => x.GetWithRolesAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _fixture.SetupStudentPermissions(userId, courseId);
            _fixture.SetupLiveKitToken(expectedToken);

            // Act
            var result = await _fixture.LivestreamService.GenerateJoinTokenAsync(userId, livestreamId);

            // Assert
            result.Should().NotBeNull();
            result.Token.Should().Be(expectedToken);

            _fixture.MockLiveKitService.Verify(x => x.GenerateToken(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                ParticipantRole.Student, // role should be Student
                It.IsAny<TimeSpan?>(), // ttl
                It.IsAny<Dictionary<string, string>?>() // attributes
            ), Times.Once);

            _fixture.ResetMocks();
        }

        [Fact]
        public async Task GenerateJoinTokenAsync_WithNonExistentLivestream_ShouldThrowNotFoundException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var livestreamId = Guid.NewGuid();

            _fixture.MockLivestreamRepository
                .Setup(x => x.GetLivestreamWithDetailsAsync(livestreamId))
                .ReturnsAsync((Livestream?)null);

            // Act
            var act = async () => await _fixture.LivestreamService.GenerateJoinTokenAsync(userId, livestreamId);

            // Assert
            var exception = await act.Should().ThrowAsync<ApiException>();
            exception.Which.StatusCode.Should().Be(HttpStatusCode.NotFound);
            exception.Which.ErrorCode.Should().Be("RESOURCE_NOT_FOUND");

            _fixture.ResetMocks();
        }

        #endregion
    }
}
