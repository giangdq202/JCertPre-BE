using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.Feedback;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Entities;

namespace JCertPreApplication.Application.Features.Feedbacks
{
    /// <summary>
    /// Service for handling feedback business logic.
    /// </summary>
    public class FeedbackService : IFeedbackService
    {
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;

        public FeedbackService(
            IFeedbackRepository feedbackRepository,
            IEnrollmentRepository enrollmentRepository)
        {
            _feedbackRepository = feedbackRepository ?? throw new ArgumentNullException(nameof(feedbackRepository));
            _enrollmentRepository = enrollmentRepository ?? throw new ArgumentNullException(nameof(enrollmentRepository));
        }

        /// <summary>
        /// Get paginated feedbacks for a course, ordered by createdAt descending.
        /// </summary>
        public async Task<Pagination<FeedbackDto>> GetPagingByCourseIdAsync(Guid courseId, int pageIndex, int pageSize)
        {
            try
            {
                var page = await _feedbackRepository.GetPagingByCourseIdAsync(courseId, pageIndex, pageSize, "User");
                return new Pagination<FeedbackDto>
                {
                    PageIndex = page.PageIndex,
                    PageSize = page.PageSize,
                    TotalItemsCount = page.TotalItemsCount,
                    Items = page.Items.Select(MapToDto).ToList()
                };
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("FEEDBACK_SERVICE_ERROR", $"Error getting feedbacks: {ex.Message}");
            }
        }

        /// <summary>
        /// Create a feedback for a user and course.
        /// </summary>
        public async Task<FeedbackDto> CreateAsync(CreateFeedbackDto dto)
        {
            try
            {
                // Check enrollment
                var isEnrolled = await _enrollmentRepository.IsUserEnrolledAsync(dto.UserId, dto.CourseId);
                if (!isEnrolled)
                    throw ApiException.Forbidden("USER_NOT_ENROLLED", "User must be enrolled in the course to leave feedback.");

                var existed = await _feedbackRepository.GetByUserAndCourseAsync(dto.UserId, dto.CourseId);
                if (existed != null)
                    throw ApiException.BadRequest("FEEDBACK_EXISTS", "Feedback already exists for this user and course.");

                var feedback = new Feedback
                {
                    feedbackId = Guid.NewGuid(),
                    courseId = dto.CourseId,
                    userId = dto.UserId,
                    rating = dto.Rating,
                    comment = dto.Comment,
                    createdAt = DateTime.UtcNow
                };

                await _feedbackRepository.InsertAsync(feedback);
                await _feedbackRepository.SaveChangesAsync();

                // Reload with User navigation property for mapping
                var created = await _feedbackRepository.GetByUserAndCourseAsync(dto.UserId, dto.CourseId);
                return MapToDto(created!);
            }
            catch (ApiException) { throw; }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("FEEDBACK_SERVICE_ERROR", $"Error creating feedback: {ex.Message}");
            }
        }

        /// <summary>
        /// Update a feedback by user and course.
        /// </summary>
        public async Task<FeedbackDto> UpdateAsync(Guid userId, Guid courseId, UpdateFeedbackDto dto)
        {
            try
            {
                var feedback = await _feedbackRepository.GetByUserAndCourseAsync(userId, courseId);
                if (feedback == null)
                    throw ApiException.NotFound("Feedback", $"userId={userId}, courseId={courseId}");

                // Only update fields that are provided by the user
                if (dto.Rating.HasValue)
                    feedback.rating = dto.Rating.Value;

                if (dto.Comment != null)
                    feedback.comment = dto.Comment;

                await _feedbackRepository.UpdateAsync(feedback);
                await _feedbackRepository.SaveChangesAsync();

                // Reload with User navigation property for mapping
                var updated = await _feedbackRepository.GetByUserAndCourseAsync(userId, courseId);
                return MapToDto(updated!);
            }
            catch (ApiException) { throw; }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("FEEDBACK_SERVICE_ERROR", $"Error updating feedback: {ex.Message}");
            }
        }

        /// <summary>
        /// Delete a feedback by user and course.
        /// </summary>
        public async Task DeleteAsync(Guid userId, Guid courseId)
        {
            try
            {
                var feedback = await _feedbackRepository.GetByUserAndCourseAsync(userId, courseId);
                if (feedback == null)
                    throw ApiException.NotFound("Feedback", $"userId={userId}, courseId={courseId}");

                await _feedbackRepository.DeleteAsync(feedback);
                await _feedbackRepository.SaveChangesAsync();
            }
            catch (ApiException) { throw; }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("FEEDBACK_SERVICE_ERROR", $"Error deleting feedback: {ex.Message}");
            }
        }

        /// <summary>
        /// Get average rating for a course.
        /// </summary>
        public async Task<decimal> GetCourseAverageRatingAsync(Guid courseId)
        {
            try
            {
                return await _feedbackRepository.GetCourseAverageRatingAsync(courseId);
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("FEEDBACK_SERVICE_ERROR", $"Error getting average rating: {ex.Message}");
            }
        }

        private static FeedbackDto MapToDto(Feedback f) => new FeedbackDto
        {
            FeedbackId = f.feedbackId,
            CourseId = f.courseId,
            UserId = f.userId,
            Rating = f.rating,
            Comment = f.comment,
            CreatedAt = f.createdAt,
            UserFullName = f.User?.fullName,
            UserAvatarUrl = f.User?.avatarUrl
        };
    }
}