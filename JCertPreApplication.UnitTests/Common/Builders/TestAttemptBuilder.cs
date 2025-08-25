using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.UnitTests.Common.Builders;

public class TestAttemptBuilder
{
    private readonly TestAttempt _testAttempt;

    public TestAttemptBuilder()
    {
        _testAttempt = new TestAttempt
        {
            attemptId = Guid.NewGuid(),
            userId = Guid.NewGuid(),
            testId = Guid.NewGuid(),
            startTime = DateTime.UtcNow,
            endTime = DateTime.UtcNow.AddMinutes(60),
            attemptNumber = 1,
            status = TestAttemptStatus.InProgress,
            isPass = null
        };
    }

    public static TestAttemptBuilder Create() => new TestAttemptBuilder();

    public TestAttemptBuilder WithId(Guid id)
    {
        _testAttempt.attemptId = id;
        return this;
    }

    public TestAttemptBuilder WithUserId(Guid userId)
    {
        _testAttempt.userId = userId;
        return this;
    }

    public TestAttemptBuilder WithTestId(Guid testId)
    {
        _testAttempt.testId = testId;
        return this;
    }

    public TestAttemptBuilder WithStatus(TestAttemptStatus status)
    {
        _testAttempt.status = status;
        return this;
    }

    public TestAttemptBuilder WithAttemptNumber(int attemptNumber)
    {
        _testAttempt.attemptNumber = attemptNumber;
        return this;
    }

    public TestAttemptBuilder WithStartTime(DateTime startTime)
    {
        _testAttempt.startTime = startTime;
        return this;
    }

    public TestAttemptBuilder WithEndTime(DateTime endTime)
    {
        _testAttempt.endTime = endTime;
        return this;
    }

    public TestAttemptBuilder WithIsPass(bool? isPass)
    {
        _testAttempt.isPass = isPass;
        return this;
    }

    public TestAttemptBuilder WithUser(User user)
    {
        _testAttempt.userId = user.userId;
        _testAttempt.User = user;
        return this;
    }

    public TestAttemptBuilder WithTest(Test test)
    {
        _testAttempt.testId = test.testId;
        _testAttempt.Test = test;
        return this;
    }

    public TestAttempt Build() => _testAttempt;
}
