using JCertPreApplication.Domain.Entities;

namespace JCertPreApplication.UnitTests.Common.Builders
{
    /// <summary>
    /// Builder pattern for creating Feedback test data
    /// </summary>
    public class FeedbackBuilder
    {
        private Feedback _feedback;

        public FeedbackBuilder()
        {
            _feedback = new Feedback
            {
                feedbackId = Guid.NewGuid(),
                courseId = Guid.NewGuid(),
                userId = Guid.NewGuid(),
                rating = 4.5m,
                comment = "Great course!",
                createdAt = DateTime.UtcNow
            };
        }

        public static FeedbackBuilder Create() => new FeedbackBuilder();

        public FeedbackBuilder WithId(Guid id)
        {
            _feedback.feedbackId = id;
            return this;
        }

        public FeedbackBuilder WithCourseId(Guid courseId)
        {
            _feedback.courseId = courseId;
            return this;
        }

        public FeedbackBuilder WithUserId(Guid userId)
        {
            _feedback.userId = userId;
            return this;
        }

        public FeedbackBuilder WithRating(decimal rating)
        {
            _feedback.rating = rating;
            return this;
        }

        public FeedbackBuilder WithComment(string? comment)
        {
            _feedback.comment = comment;
            return this;
        }

        public FeedbackBuilder WithCreatedAt(DateTime createdAt)
        {
            _feedback.createdAt = createdAt;
            return this;
        }

        public FeedbackBuilder WithUser(User user)
        {
            _feedback.User = user;
            return this;
        }

        public Feedback Build() => _feedback;
    }
}
