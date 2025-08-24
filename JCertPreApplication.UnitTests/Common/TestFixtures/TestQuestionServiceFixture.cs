using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Features.TestQuestions;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;
using JCertPreApplication.UnitTests.Common.Builders;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JCertPreApplication.UnitTests.Common.TestFixtures;

public class TestQuestionServiceFixture
{
    public TestQuestionService TestQuestionService { get; }
    public Mock<ITestQuestionRepository> MockTestQuestionRepository { get; }
    public Mock<ITestRepository> MockTestRepository { get; }
    public Mock<ITestAttemptRepository> MockTestAttemptRepository { get; }
    public Mock<IQuestionRepository> MockQuestionRepository { get; }
    public Mock<ITestTemplateTypeRepository> MockTestTemplateTypeRepository { get; }
    public Mock<ITestTemplateRepository> MockTestTemplateRepository { get; }
    public Mock<ITestTemplateConfigRepository> MockTestTemplateConfigRepository { get; }

    public TestQuestionServiceFixture()
    {
        MockTestQuestionRepository = new Mock<ITestQuestionRepository>();
        MockTestRepository = new Mock<ITestRepository>();
        MockTestAttemptRepository = new Mock<ITestAttemptRepository>();
        MockQuestionRepository = new Mock<IQuestionRepository>();
        MockTestTemplateTypeRepository = new Mock<ITestTemplateTypeRepository>();
        MockTestTemplateRepository = new Mock<ITestTemplateRepository>();
        MockTestTemplateConfigRepository = new Mock<ITestTemplateConfigRepository>();

        TestQuestionService = new TestQuestionService(
            MockTestQuestionRepository.Object,
            MockTestRepository.Object,
            MockTestAttemptRepository.Object,
            MockQuestionRepository.Object,
            MockTestTemplateTypeRepository.Object,
            MockTestTemplateRepository.Object,
            MockTestTemplateConfigRepository.Object
        );
    }

    public static List<(Guid testId, Guid questionId)> CreateTestQuestionPairs(Guid testId, int count)
    {
        var pairs = new List<(Guid, Guid)>();
        for (int i = 0; i < count; i++)
        {
            pairs.Add((testId, Guid.NewGuid()));
        }
        return pairs;
    }

    public static List<TestQuestion> CreateTestQuestionsForTest(Guid testId, int count, int startNumber = 1)
    {
        var testQuestions = new List<TestQuestion>();
        for (int i = 0; i < count; i++)
        {
            testQuestions.Add(TestQuestionBuilder.Create()
                .WithTestId(testId)
                .WithQuestionNumber(startNumber + i)
                .Build());
        }
        return testQuestions;
    }

    public static Test CreateTestWithStatus(TestStatus status, Guid? testId = null)
    {
        return TestBuilder.Create()
            .WithId(testId ?? Guid.NewGuid())
            .WithStatus(status)
            .Build();
    }

    public static Question CreateActiveQuestion(Guid? questionId = null)
    {
        return QuestionBuilder.Create()
            .WithId(questionId ?? Guid.NewGuid())
            .WithIsActive(true)
            .Build();
    }

    public static Question CreateInactiveQuestion(Guid? questionId = null)
    {
        return QuestionBuilder.Create()
            .WithId(questionId ?? Guid.NewGuid())
            .WithIsActive(false)
            .Build();
    }

    public static TestAttempt CreateActiveTestAttempt(Guid testId, DateTime now)
    {
        return TestAttemptBuilder.Create()
            .WithTestId(testId)
            .WithStartTime(now.AddMinutes(-30))
            .WithEndTime(now.AddMinutes(30))
            .Build();
    }

    public static TestAttempt CreateInactiveTestAttempt(Guid testId, DateTime now)
    {
        return TestAttemptBuilder.Create()
            .WithTestId(testId)
            .WithStartTime(now.AddHours(-2))
            .WithEndTime(now.AddHours(-1))
            .Build();
    }
}
