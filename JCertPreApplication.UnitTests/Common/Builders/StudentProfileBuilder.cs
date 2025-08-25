using JCertPreApplication.Domain.Entities;

namespace JCertPreApplication.UnitTests.Common.Builders
{
    public class StudentProfileBuilder
    {
        private StudentProfile _studentProfile;

        public StudentProfileBuilder()
        {
            _studentProfile = new StudentProfile
            {
                userId = Guid.NewGuid(),
                currentLevel = "N3",
                learningGoals = "Pass JLPT N2 within 6 months",
                numberOfTestsTaken = 0,
                lastResetTestTime = null
            };
        }

        public static StudentProfileBuilder Create() => new StudentProfileBuilder();

        public StudentProfileBuilder WithUserId(Guid userId)
        {
            _studentProfile.userId = userId;
            return this;
        }

        public StudentProfileBuilder WithCurrentLevel(string currentLevel)
        {
            _studentProfile.currentLevel = currentLevel;
            return this;
        }

        public StudentProfileBuilder WithLearningGoals(string learningGoals)
        {
            _studentProfile.learningGoals = learningGoals;
            return this;
        }

        public StudentProfileBuilder WithNumberOfTestsTaken(int numberOfTestsTaken)
        {
            _studentProfile.numberOfTestsTaken = numberOfTestsTaken;
            return this;
        }

        public StudentProfileBuilder WithLastResetTestTime(DateTime? lastResetTestTime)
        {
            _studentProfile.lastResetTestTime = lastResetTestTime;
            return this;
        }

        public StudentProfile Build() => _studentProfile;
    }
}
