using JCertPreApplication.Domain.Entities;
using System;

namespace JCertPreApplication.UnitTests.Common.Builders;

public class TestQuestionBuilder
{
    private TestQuestion _testQuestion;

    public TestQuestionBuilder()
    {
        _testQuestion = new TestQuestion
        {
            testQuestionId = Guid.NewGuid(),
            testId = Guid.NewGuid(),
            questionId = Guid.NewGuid(),
            questionNumber = 1,
            partNumber = null,
            partDurationMinutes = null
        };
    }

    public static TestQuestionBuilder Create() => new TestQuestionBuilder();

    public TestQuestionBuilder WithId(Guid testQuestionId)
    {
        _testQuestion.testQuestionId = testQuestionId;
        return this;
    }

    public TestQuestionBuilder WithTestId(Guid testId)
    {
        _testQuestion.testId = testId;
        return this;
    }

    public TestQuestionBuilder WithQuestionId(Guid questionId)
    {
        _testQuestion.questionId = questionId;
        return this;
    }

    public TestQuestionBuilder WithQuestionNumber(int questionNumber)
    {
        _testQuestion.questionNumber = questionNumber;
        return this;
    }

    public TestQuestionBuilder WithPartNumber(int? partNumber)
    {
        _testQuestion.partNumber = partNumber;
        return this;
    }

    public TestQuestionBuilder WithPartDuration(int? partDurationMinutes)
    {
        _testQuestion.partDurationMinutes = partDurationMinutes;
        return this;
    }

    public TestQuestion Build() => _testQuestion;
}
