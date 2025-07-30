using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.Livestream;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.Application.Features.Livestreams
{
    public class LivestreamService : ILivestreamService
    {
        private readonly ILivestreamRepository _livestreamRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly ICourseInstructorRepository _courseInstructorRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILiveKitService _liveKitService;

        public LivestreamService(
            ILivestreamRepository livestreamRepository,
            ICourseRepository courseRepository,
            IEnrollmentRepository enrollmentRepository,
            ICourseInstructorRepository courseInstructorRepository,
            IUserRepository userRepository,
            ILiveKitService liveKitService)
        {
            _livestreamRepository = livestreamRepository;
            _courseRepository = courseRepository;
            _enrollmentRepository = enrollmentRepository;
            _courseInstructorRepository = courseInstructorRepository;
            _userRepository = userRepository;
            _liveKitService = liveKitService;
        }

        public async Task<LivestreamDto> CreateLivestreamAsync(CreateLivestreamDto createDto)
        {
            try
            {
                // Validate course exists
                var course = await _courseRepository.GetByIdAsync(createDto.CourseId);
                if (course == null)
                    throw ApiException.NotFound("Course", createDto.CourseId);

                // Validate scheduled time is not in the past
                if (createDto.ScheduledDateTime <= DateTime.UtcNow)
                    throw ApiException.BadRequest("INVALID_SCHEDULE_TIME", "Scheduled time must be in the future");

                // Check for schedule conflicts
                var hasConflict = await _livestreamRepository.HasScheduleConflictAsync(
                    createDto.CourseId, 
                    createDto.ScheduledDateTime, 
                    createDto.DurationMinutes);
                
                if (hasConflict)
                    throw ApiException.BadRequest("SCHEDULE_CONFLICT", "There is already a livestream scheduled at this time for this course");

                var livestream = new Livestream
                {
                    livestreamId = Guid.NewGuid(),
                    courseId = createDto.CourseId,
                    description = createDto.Description,
                    scheduledDateTime = createDto.ScheduledDateTime,
                    durationMinutes = createDto.DurationMinutes,
                    status = LivestreamStatus.SCHEDULED
                };

                await _livestreamRepository.InsertAsync(livestream);
                await _livestreamRepository.SaveChangesAsync();

                var created = await _livestreamRepository.GetLivestreamWithDetailsAsync(livestream.livestreamId);
                return MapToDto(created!);
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("CREATE_LIVESTREAM_ERROR", ex.Message);
            }
        }

        public async Task<LivestreamDto?> GetLivestreamByIdAsync(Guid livestreamId)
        {
            try
            {
                var livestream = await _livestreamRepository.GetLivestreamWithDetailsAsync(livestreamId);
                return livestream != null ? MapToDto(livestream) : null;
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("GET_LIVESTREAM_ERROR", ex.Message);
            }
        }

        public async Task<LivestreamDto> UpdateLivestreamAsync(Guid livestreamId, UpdateLivestreamDto updateDto)
        {
            try
            {
                var livestream = await _livestreamRepository.GetByIdAsync(livestreamId);
                if (livestream == null)
                    throw ApiException.NotFound("Livestream", livestreamId);

                // Don't allow updates if livestream is already live or completed
                if (livestream.status == LivestreamStatus.LIVE)
                    throw ApiException.BadRequest("LIVESTREAM_ACTIVE", "Cannot update active livestream");

                if (livestream.status == LivestreamStatus.COMPLETED)
                    throw ApiException.BadRequest("LIVESTREAM_COMPLETED", "Cannot update completed livestream");

                // Update fields if provided
                if (!string.IsNullOrEmpty(updateDto.Description))
                    livestream.description = updateDto.Description;

                if (updateDto.ScheduledDateTime.HasValue)
                {
                    if (updateDto.ScheduledDateTime.Value <= DateTime.UtcNow)
                        throw ApiException.BadRequest("INVALID_SCHEDULE_TIME", "Scheduled time must be in the future");

                    // Check for conflicts when updating schedule
                    var hasConflict = await _livestreamRepository.HasScheduleConflictAsync(
                        livestream.courseId,
                        updateDto.ScheduledDateTime.Value,
                        updateDto.DurationMinutes ?? livestream.durationMinutes,
                        livestreamId);

                    if (hasConflict)
                        throw ApiException.BadRequest("SCHEDULE_CONFLICT", "There is already a livestream scheduled at this time for this course");

                    livestream.scheduledDateTime = updateDto.ScheduledDateTime.Value;
                }

                if (updateDto.DurationMinutes.HasValue)
                    livestream.durationMinutes = updateDto.DurationMinutes.Value;

                await _livestreamRepository.UpdateAsync(livestream);
                await _livestreamRepository.SaveChangesAsync();

                var updated = await _livestreamRepository.GetLivestreamWithDetailsAsync(livestreamId);
                return MapToDto(updated!);
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("UPDATE_LIVESTREAM_ERROR", ex.Message);
            }
        }

        public async Task DeleteLivestreamAsync(Guid livestreamId)
        {
            try
            {
                var livestream = await _livestreamRepository.GetByIdAsync(livestreamId);
                if (livestream == null)
                    throw ApiException.NotFound("Livestream", livestreamId);

                // Don't allow deletion if livestream is live
                if (livestream.status == LivestreamStatus.LIVE)
                    throw ApiException.BadRequest("LIVESTREAM_ACTIVE", "Cannot delete active livestream");

                await _livestreamRepository.DeleteAsync(livestream);
                await _livestreamRepository.SaveChangesAsync();
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("DELETE_LIVESTREAM_ERROR", ex.Message);
            }
        }

        public async Task<Pagination<LivestreamDto>> GetLivestreamsAsync(
            Guid? courseId = null,
            string? searchTerm = null,
            int pageIndex = 1,
            int pageSize = 10)
        {
            try
            {
                var paginatedLivestreams = await _livestreamRepository.GetLivestreamsWithPaginationAsync(
                    courseId, searchTerm, pageIndex, pageSize);

                var dtos = paginatedLivestreams.Items.Select(MapToDto).ToList();

                return new Pagination<LivestreamDto>
                {
                    PageIndex = paginatedLivestreams.PageIndex,
                    PageSize = paginatedLivestreams.PageSize,
                    TotalItemsCount = paginatedLivestreams.TotalItemsCount,
                    Items = dtos
                };
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("GET_LIVESTREAMS_ERROR", ex.Message);
            }
        }

        public async Task<List<LivestreamDto>> GetLivestreamsByCourseAsync(Guid courseId)
        {
            try
            {
                var livestreams = await _livestreamRepository.GetLivestreamsByCourseIdAsync(courseId);
                return livestreams.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("GET_COURSE_LIVESTREAMS_ERROR", ex.Message);
            }
        }

        public async Task<List<LivestreamDto>> GetLivestreamsByUserAsync(Guid userId)
        {
            try
            {
                var livestreams = await _livestreamRepository.GetLivestreamsByUserAsync(userId);
                return livestreams.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("GET_USER_LIVESTREAMS_ERROR", ex.Message);
            }
        }

        public async Task<List<LivestreamTimetableDto>> GetLivestreamTimetableByUserAsync(Guid userId)
        {
            try
            {
                var livestreams = await _livestreamRepository.GetLivestreamsByUserAsync(userId);
                var timetableItems = new List<LivestreamTimetableDto>();

                foreach (var livestream in livestreams)
                {
                    var canJoin = await CanUserJoinLivestreamAsync(userId, livestream.livestreamId);
                    var canStart = await CanInstructorStartLivestreamAsync(userId, livestream.livestreamId);
                    var userRole = await DetermineUserRoleInCourseAsync(userId, livestream.courseId);

                    timetableItems.Add(new LivestreamTimetableDto
                    {
                        LivestreamId = livestream.livestreamId,
                        CourseId = livestream.courseId,
                        CourseName = livestream.Course?.title ?? "Unknown Course",
                        Description = livestream.description,
                        ScheduledDateTime = livestream.scheduledDateTime,
                        DurationMinutes = livestream.durationMinutes,
                        Status = livestream.status,
                        CanJoin = canJoin,
                        CanStart = canStart,
                        UserRole = userRole
                    });
                }

                return timetableItems.OrderBy(x => x.ScheduledDateTime).ToList();
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("GET_USER_TIMETABLE_ERROR", ex.Message);
            }
        }

        public async Task<LivestreamJoinDto> GenerateJoinTokenAsync(Guid userId, Guid livestreamId)
        {
            try
            {
                var livestream = await _livestreamRepository.GetLivestreamWithDetailsAsync(livestreamId);
                if (livestream == null)
                    throw ApiException.NotFound("Livestream", livestreamId);

                var user = await _userRepository.GetWithRolesAsync(userId);
                if (user == null)
                    throw ApiException.NotFound("User", userId);

                // Determine participant role for LiveKit
                var role = await DetermineParticipantRoleAsync(userId, livestream.courseId);

                // Generate token using LiveKit service
                var token = _liveKitService.GenerateToken(
                    roomName: GetRoomName(livestreamId),
                    participantIdentity: userId.ToString(),
                    participantName: user.fullName,
                    role: role
                );

                return new LivestreamJoinDto
                {
                    Token = token,
                    RoomName = GetRoomName(livestreamId),
                    Title = GetDisplayTitle(MapToDto(livestream)),
                    ScheduledDateTime = livestream.scheduledDateTime,
                    Description = livestream.description,
                    DurationMinutes = livestream.durationMinutes
                };
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("GENERATE_TOKEN_ERROR", ex.Message);
            }
        }

        public async Task<bool> CanUserJoinLivestreamAsync(Guid userId, Guid livestreamId)
        {
            try
            {
                var livestream = await _livestreamRepository.GetByIdAsync(livestreamId);
                if (livestream == null) return false;

                var user = await _userRepository.GetWithRolesAsync(userId);
                if (user == null) return false;

                // Check if user is instructor of the course
                if (await _courseInstructorRepository.IsInstructorAssignedToCourse(livestream.courseId, userId))
                    return true;

                // Check if user is enrolled in the course
                if (await _enrollmentRepository.IsUserEnrolledAsync(userId, livestream.courseId))
                    return true;

                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CanInstructorStartLivestreamAsync(Guid userId, Guid livestreamId)
        {
            try
            {
                var livestream = await _livestreamRepository.GetByIdAsync(livestreamId);
                if (livestream == null) return false;

                return await _courseInstructorRepository.IsInstructorAssignedToCourse(livestream.courseId, userId);
            }
            catch
            {
                return false;
            }
        }

        public async Task StartLivestreamAsync(Guid livestreamId)
        {
            try
            {
                var livestream = await _livestreamRepository.GetByIdAsync(livestreamId);
                if (livestream == null)
                    throw ApiException.NotFound("Livestream", livestreamId);

                if (livestream.status != LivestreamStatus.SCHEDULED)
                    throw ApiException.BadRequest("INVALID_STATUS", "Only scheduled livestreams can be started");

                // Create LiveKit room
                await _liveKitService.CreateRoomAsync(GetRoomName(livestreamId));

                // Update status
                livestream.status = LivestreamStatus.LIVE;
                await _livestreamRepository.UpdateAsync(livestream);
                await _livestreamRepository.SaveChangesAsync();
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("START_LIVESTREAM_ERROR", ex.Message);
            }
        }

        public async Task EndLivestreamAsync(Guid livestreamId)
        {
            try
            {
                var livestream = await _livestreamRepository.GetByIdAsync(livestreamId);
                if (livestream == null)
                    throw ApiException.NotFound("Livestream", livestreamId);

                if (livestream.status != LivestreamStatus.LIVE)
                    throw ApiException.BadRequest("INVALID_STATUS", "Only live livestreams can be ended");

                // Delete LiveKit room
                await _liveKitService.DeleteRoomAsync(GetRoomName(livestreamId));

                // Update status
                livestream.status = LivestreamStatus.COMPLETED;
                await _livestreamRepository.UpdateAsync(livestream);
                await _livestreamRepository.SaveChangesAsync();
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("END_LIVESTREAM_ERROR", ex.Message);
            }
        }

        public string GetDisplayTitle(LivestreamDto livestream)
        {
            return $"{livestream.CourseName} - {livestream.ScheduledDateTime:dd/MM/yyyy HH:mm}";
        }

        public string GetRoomName(Guid livestreamId)
        {
            return livestreamId.ToString();
        }

        private async Task<ParticipantRole> DetermineParticipantRoleAsync(Guid userId, Guid courseId)
        {
            if (await _courseInstructorRepository.IsInstructorAssignedToCourse(courseId, userId))
                return ParticipantRole.Instructor;

            return ParticipantRole.Student;
        }

        private async Task<UserRoleInCourse> DetermineUserRoleInCourseAsync(Guid userId, Guid courseId)
        {
            if (await _courseInstructorRepository.IsInstructorAssignedToCourse(courseId, userId))
                return UserRoleInCourse.Instructor;

            if (await _enrollmentRepository.IsUserEnrolledAsync(userId, courseId))
                return UserRoleInCourse.Student;

            return UserRoleInCourse.None;
        }

        private static LivestreamDto MapToDto(Livestream livestream)
        {
            return new LivestreamDto
            {
                LivestreamId = livestream.livestreamId,
                CourseId = livestream.courseId,
                Description = livestream.description,
                ScheduledDateTime = livestream.scheduledDateTime,
                DurationMinutes = livestream.durationMinutes,
                Status = livestream.status,
                CourseName = livestream.Course?.title
            };
        }
    }
}
