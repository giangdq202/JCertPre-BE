using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.Feedback;
using JCertPreApplication.Application.Features.Feedbacks;
using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.UnitTests.Common.Builders;
using Moq;

namespace JCertPreApplication.UnitTests.Common.TestFixtures
{
    /// <summary>
    /// Test fixture for FeedbackService providing mocked dependencies and helper methods
    /// </summary>
    public class FeedbackServiceFixture
    {
        public FeedbackService FeedbackService { get; }
        public Mock<IFeedbackRepository> MockFeedbackRepository { get; }
        public Mock<IEnrollmentRepository> MockEnrollmentRepository { get; }

        public FeedbackServiceFixture()
        {
            MockFeedbackRepository = new Mock<IFeedbackRepository>();
            MockEnrollmentRepository = new Mock<IEnrollmentRepository>();

            FeedbackService = new FeedbackService(
                MockFeedbackRepository.Object,
                MockEnrollmentRepository.Object
            );
        }

        /// <summary>
        /// Creates a valid CreateFeedbackDto for testing
        /// </summary>
        public static CreateFeedbackDto ValidCreateDto(Guid? courseId = null, Guid? userId = null)
        {
            return new CreateFeedbackDto
            {
                CourseId = courseId ?? Guid.NewGuid(),
                UserId = userId ?? Guid.NewGuid(),
                Rating = 4.5m,
                Comment = "Great course! Very informative and well structured."
            };
        }

        /// <summary>
        /// Creates a valid UpdateFeedbackDto for testing
        /// </summary>
        public static UpdateFeedbackDto ValidUpdateDto()
        {
            return new UpdateFeedbackDto
            {
                Rating = 5.0m,
                Comment = "Updated comment - excellent course!"
            };
        }

        /// <summary>
        /// Creates an UpdateFeedbackDto with only rating
        /// </summary>
        public static UpdateFeedbackDto UpdateDtoWithRatingOnly()
        {
            return new UpdateFeedbackDto
            {
                Rating = 3.0m,
                Comment = null
            };
        }

        /// <summary>
        /// Creates an UpdateFeedbackDto with only comment
        /// </summary>
        public static UpdateFeedbackDto UpdateDtoWithCommentOnly()
        {
            return new UpdateFeedbackDto
            {
                Rating = null,
                Comment = "Updated comment only"
            };
        }

        /// <summary>
        /// Creates a list of feedback entities for testing
        /// </summary>
        public static List<Feedback> CreateFeedbackList(Guid courseId, int count)
        {
            var feedbacks = new List<Feedback>();
            
            for (int i = 0; i < count; i++)
            {
                var user = UserBuilder.Create()
                    .WithId(Guid.NewGuid())
                    .WithName($"User {i + 1}")
                    .WithAvatarUrl($"https://example.com/avatar{i + 1}.jpg")
                    .Build();

                var feedback = FeedbackBuilder.Create()
                    .WithCourseId(courseId)
                    .WithUserId(user.userId)
                    .WithRating(4.0m + (i % 2)) // Alternating between 4.0 and 5.0
                    .WithComment($"Feedback comment {i + 1}")
                    .WithUser(user)
                    .Build();
                
                feedbacks.Add(feedback);
            }
            
            return feedbacks;
        }

        /// <summary>
        /// Creates a paginated result for testing
        /// </summary>
        public static Pagination<Feedback> CreatePaginatedFeedbacks(
            List<Feedback> feedbacks, 
            int pageIndex, 
            int pageSize, 
            int totalCount)
        {
            return new Pagination<Feedback>
            {
                Items = feedbacks,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalItemsCount = totalCount
            };
        }

        /// <summary>
        /// Creates a feedback with user navigation property for testing
        /// </summary>
        public static Feedback CreateFeedbackWithUser(Guid? courseId = null, Guid? userId = null)
        {
            var user = UserBuilder.Create()
                .WithId(userId ?? Guid.NewGuid())
                .WithName("John Doe")
                .WithAvatarUrl("https://example.com/avatar.jpg")
                .Build();

            return FeedbackBuilder.Create()
                .WithCourseId(courseId ?? Guid.NewGuid())
                .WithUserId(user.userId)
                .WithRating(4.5m)
                .WithComment("Great course!")
                .WithUser(user)
                .Build();
        }
    }
}
