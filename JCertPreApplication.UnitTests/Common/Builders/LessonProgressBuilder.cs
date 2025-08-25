using JCertPreApplication.Domain.Entities;

namespace JCertPreApplication.UnitTests.Common.Builders
{
    public class LessonProgressBuilder
    {
        private LessonProgress _lessonProgress;

        public LessonProgressBuilder()
        {
            _lessonProgress = new LessonProgress
            {
                progressId = Guid.NewGuid(),
                userId = Guid.NewGuid(),
                lessonId = Guid.NewGuid(),
                completionRate = 75.0m
            };
        }

        public static LessonProgressBuilder Create() => new LessonProgressBuilder();

        public LessonProgressBuilder WithId(Guid id)
        {
            _lessonProgress.progressId = id;
            return this;
        }

        public LessonProgressBuilder WithUserId(Guid userId)
        {
            _lessonProgress.userId = userId;
            return this;
        }

        public LessonProgressBuilder WithLessonId(Guid lessonId)
        {
            _lessonProgress.lessonId = lessonId;
            return this;
        }

        public LessonProgressBuilder WithCompletionRate(decimal rate)
        {
            _lessonProgress.completionRate = rate;
            return this;
        }

        public LessonProgressBuilder WithUser(User user)
        {
            _lessonProgress.User = user;
            _lessonProgress.userId = user.userId;
            return this;
        }

        public LessonProgressBuilder WithLesson(Lesson lesson)
        {
            _lessonProgress.Lesson = lesson;
            _lessonProgress.lessonId = lesson.lessonId;
            return this;
        }

        public LessonProgress Build() => _lessonProgress;
    }
}
