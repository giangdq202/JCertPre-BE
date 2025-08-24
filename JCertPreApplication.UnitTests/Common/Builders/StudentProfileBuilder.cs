using JCertPreApplication.Domain.Entities;

namespace JCertPreApplication.UnitTests.Common.Builders;

public class StudentProfileBuilder
{
    private StudentProfile _studentProfile;

    public StudentProfileBuilder()
    {
        _studentProfile = new StudentProfile
        {
            userId = Guid.NewGuid(),
            currentLevel = "N5",
            learningGoals = "Pass JLPT N5",
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

    public StudentProfileBuilder WithCurrentLevel(string level)
    {
        _studentProfile.currentLevel = level;
        return this;
    }

    public StudentProfileBuilder WithNumberOfTestsTaken(int numberOfTests)
    {
        _studentProfile.numberOfTestsTaken = numberOfTests;
        return this;
    }

    public StudentProfileBuilder WithLastResetTestTime(DateTime? lastResetTime)
    {
        _studentProfile.lastResetTestTime = lastResetTime;
        return this;
    }

    public StudentProfile Build() => _studentProfile;
}
